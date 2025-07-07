from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from motor.motor_asyncio import AsyncIOMotorClient
from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
from datetime import datetime
import os
from dotenv import load_dotenv
import uuid
import json
from bson import ObjectId

load_dotenv()

# Custom JSON encoder to handle MongoDB ObjectId
class JSONEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, ObjectId):
            return str(o)
        return super().default(o)

# Custom ObjectId for Pydantic
class PyObjectId(ObjectId):
    @classmethod
    def __get_validators__(cls):
        yield cls.validate

    @classmethod
    def validate(cls, v):
        if not ObjectId.is_valid(v):
            raise ValueError('Invalid ObjectId')
        return ObjectId(v)

    @classmethod
    def __get_pydantic_json_schema__(cls, field_schema):
        field_schema.update(type="string")

# Custom response model for MongoDB documents
def serialize_mongo_doc(doc: dict) -> dict:
    """Convert MongoDB document to serializable dict"""
    if doc is None:
        return None
    
    result = {}
    for key, value in doc.items():
        if isinstance(value, ObjectId):
            result[key] = str(value)
        elif isinstance(value, list):
            result[key] = [serialize_mongo_doc(item) if isinstance(item, dict) else 
                          str(item) if isinstance(item, ObjectId) else item 
                          for item in value]
        elif isinstance(value, dict):
            result[key] = serialize_mongo_doc(value)
        else:
            result[key] = value
    return result

app = FastAPI(title="Ristorante Manager API", version="1.0.0")

# CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Database connection
MONGO_URL = os.getenv("MONGO_URL", "mongodb://localhost:27017/ristorante_manager")
client = AsyncIOMotorClient(MONGO_URL)
db = client.ristorante_manager

# Pydantic models
class Table(BaseModel):
    id: str
    number: int
    status: str  # "free" or "occupied"
    covers: int
    use_count: int
    is_closed: bool

class Product(BaseModel):
    id: str
    name: str
    price: float
    category: str  # antipasti, pasta, pizza, dessert
    type: str  # kitchen, pizzeria
    is_customizable: bool

class DoughType(BaseModel):
    id: str
    name: str
    additional_price: float

class Extra(BaseModel):
    id: str
    name: str
    price: float

class OrderItemExtra(BaseModel):
    id: str
    order_item_id: str
    name: str
    price: float

class OrderItem(BaseModel):
    id: str
    order_id: str
    product_id: str
    name: str
    price: float
    total_price: float
    dough_type: Optional[str] = None
    product_type: str
    extras: List[OrderItemExtra] = []

class Order(BaseModel):
    id: str
    table_id: str
    created_at: datetime
    is_sent: bool
    is_closed: bool
    items: List[OrderItem] = []

# Request models
class OpenTableRequest(BaseModel):
    table_id: str
    covers: int

class AddItemRequest(BaseModel):
    table_id: str
    product_id: str
    dough_type: Optional[str] = None
    extra_ids: List[str] = []

# Initialize database with seed data
async def init_database():
    # Check if data already exists
    if await db.tables.count_documents({}) > 0:
        return
    
    # Seed tables
    tables = []
    for i in range(1, 13):
        table = {
            "id": str(uuid.uuid4()),
            "number": i,
            "status": "free",
            "covers": 0,
            "use_count": 0,
            "is_closed": False
        }
        tables.append(table)
    
    await db.tables.insert_many(tables)
    
    # Seed products
    products = [
        # Antipasti
        {"id": str(uuid.uuid4()), "name": "Bruschetta al Pomodoro", "price": 8.0, "category": "antipasti", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Antipasto Misto", "price": 12.0, "category": "antipasti", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Caprese", "price": 10.0, "category": "antipasti", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Frittura di Mare", "price": 12.0, "category": "antipasti", "type": "kitchen", "is_customizable": False},
        
        # Pasta
        {"id": str(uuid.uuid4()), "name": "Spaghetti alla Carbonara", "price": 14.0, "category": "pasta", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Penne all'Arrabbiata", "price": 14.0, "category": "pasta", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Tagliatelle ai Funghi Porcini", "price": 16.0, "category": "pasta", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Risotto ai Frutti di Mare", "price": 18.0, "category": "pasta", "type": "kitchen", "is_customizable": False},
        
        # Pizza
        {"id": str(uuid.uuid4()), "name": "Margherita", "price": 9.0, "category": "pizza", "type": "pizzeria", "is_customizable": True},
        {"id": str(uuid.uuid4()), "name": "Diavola", "price": 11.0, "category": "pizza", "type": "pizzeria", "is_customizable": True},
        {"id": str(uuid.uuid4()), "name": "Quattro Formaggi", "price": 12.0, "category": "pizza", "type": "pizzeria", "is_customizable": True},
        {"id": str(uuid.uuid4()), "name": "Capricciosa", "price": 13.0, "category": "pizza", "type": "pizzeria", "is_customizable": True},
        {"id": str(uuid.uuid4()), "name": "Napoletana", "price": 10.0, "category": "pizza", "type": "pizzeria", "is_customizable": True},
        {"id": str(uuid.uuid4()), "name": "Prosciutto e Funghi", "price": 12.0, "category": "pizza", "type": "pizzeria", "is_customizable": True},
        
        # Dessert
        {"id": str(uuid.uuid4()), "name": "Tiramisù", "price": 6.0, "category": "dessert", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Panna Cotta", "price": 6.0, "category": "dessert", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Cannoli Siciliani", "price": 7.0, "category": "dessert", "type": "kitchen", "is_customizable": False},
        {"id": str(uuid.uuid4()), "name": "Gelato Artigianale", "price": 6.0, "category": "dessert", "type": "kitchen", "is_customizable": False}
    ]
    
    await db.products.insert_many(products)
    
    # Seed dough types
    dough_types = [
        {"id": str(uuid.uuid4()), "name": "Classica", "additional_price": 0.0},
        {"id": str(uuid.uuid4()), "name": "Napoli", "additional_price": 0.0},
        {"id": str(uuid.uuid4()), "name": "Cereali", "additional_price": 2.0},
        {"id": str(uuid.uuid4()), "name": "Senza Glutine", "additional_price": 2.0}
    ]
    
    await db.dough_types.insert_many(dough_types)
    
    # Seed extras
    extras = [
        {"id": str(uuid.uuid4()), "name": "Mozzarella senza lattosio", "price": 1.5},
        {"id": str(uuid.uuid4()), "name": "Bufala", "price": 2.0},
        {"id": str(uuid.uuid4()), "name": "Funghi porcini", "price": 2.5},
        {"id": str(uuid.uuid4()), "name": "Prosciutto crudo", "price": 2.0}
    ]
    
    await db.extras.insert_many(extras)

@app.on_event("startup")
async def startup_event():
    await init_database()

# API Routes

# Tables
@app.get("/api/tables")
async def get_tables():
    """Get all tables with their current status"""
    tables = await db.tables.find({"is_closed": False}).to_list(None)
    tables = [serialize_mongo_doc(table) for table in tables]
    
    # Get active orders count for each table
    for table in tables:
        orders = await db.orders.find({"table_id": table["id"], "is_closed": False}).to_list(None)
        orders = [serialize_mongo_doc(order) for order in orders]
        items_count = 0
        total = 0.0
        
        for order in orders:
            items = await db.order_items.find({"order_id": order["id"]}).to_list(None)
            items = [serialize_mongo_doc(item) for item in items]
            items_count += len(items)
            total += sum(item["total_price"] for item in items)
        
        table["items_count"] = items_count
        table["total"] = total
    
    return tables

@app.get("/api/tables/{table_id}")
async def get_table(table_id: str):
    """Get table details"""
    table = await db.tables.find_one({"id": table_id})
    if not table:
        raise HTTPException(status_code=404, detail="Table not found")
    
    table = serialize_mongo_doc(table)
    
    # Get active order
    active_order = await db.orders.find_one({"table_id": table_id, "is_closed": False})
    if active_order:
        active_order = serialize_mongo_doc(active_order)
        items = await db.order_items.find({"order_id": active_order["id"]}).to_list(None)
        items = [serialize_mongo_doc(item) for item in items]
        table["active_order"] = active_order
        table["items"] = items
    
    return table

@app.post("/api/tables/open")
async def open_table(request: OpenTableRequest):
    """Open a table with number of covers"""
    table = await db.tables.find_one({"id": request.table_id})
    if not table:
        raise HTTPException(status_code=404, detail="Table not found")
    
    # Update table status
    await db.tables.update_one(
        {"id": request.table_id},
        {
            "$set": {
                "status": "occupied",
                "covers": request.covers
            },
            "$inc": {"use_count": 1}
        }
    )
    
    # Create new order
    order = {
        "id": str(uuid.uuid4()),
        "table_id": request.table_id,
        "created_at": datetime.now(),
        "is_sent": False,
        "is_closed": False
    }
    
    await db.orders.insert_one(order)
    
    return {"message": "Table opened successfully", "order_id": order["id"]}

@app.post("/api/tables/{table_id}/close")
async def close_table(table_id: str):
    """Close a table"""
    table = await db.tables.find_one({"id": table_id})
    if not table:
        raise HTTPException(status_code=404, detail="Table not found")
    
    # Close active order
    await db.orders.update_many(
        {"table_id": table_id, "is_closed": False},
        {"$set": {"is_closed": True}}
    )
    
    # Reset table
    await db.tables.update_one(
        {"id": table_id},
        {
            "$set": {
                "status": "free",
                "covers": 0
            }
        }
    )
    
    return {"message": "Table closed successfully"}

# Products
@app.get("/api/products")
async def get_products(category: Optional[str] = None):
    """Get all products or by category"""
    filter_query = {}
    if category:
        filter_query["category"] = category
    
    products = await db.products.find(filter_query).to_list(None)
    products = [serialize_mongo_doc(product) for product in products]
    return products

@app.get("/api/products/categories")
async def get_categories():
    """Get all product categories"""
    categories = await db.products.distinct("category")
    return categories

# Dough types and extras
@app.get("/api/dough-types")
async def get_dough_types():
    """Get all dough types"""
    dough_types = await db.dough_types.find().to_list(None)
    dough_types = [serialize_mongo_doc(dough_type) for dough_type in dough_types]
    return dough_types

@app.get("/api/extras")
async def get_extras():
    """Get all extras"""
    extras = await db.extras.find().to_list(None)
    extras = [serialize_mongo_doc(extra) for extra in extras]
    return extras

# Orders
@app.post("/api/orders/add-item")
async def add_item_to_order(request: AddItemRequest):
    """Add an item to an active order"""
    # Get active order for table
    active_order = await db.orders.find_one({"table_id": request.table_id, "is_closed": False})
    if not active_order:
        raise HTTPException(status_code=404, detail="No active order found for this table")
    
    active_order = serialize_mongo_doc(active_order)
    
    # Get product
    product = await db.products.find_one({"id": request.product_id})
    if not product:
        raise HTTPException(status_code=404, detail="Product not found")
    
    product = serialize_mongo_doc(product)
    
    # Calculate total price
    total_price = product["price"]
    
    # Create order item
    order_item = {
        "id": str(uuid.uuid4()),
        "order_id": active_order["id"],
        "product_id": request.product_id,
        "name": product["name"],
        "price": product["price"],
        "product_type": product["type"],
        "dough_type": request.dough_type,
        "extras": []
    }
    
    # Add dough price if applicable
    if request.dough_type:
        dough = await db.dough_types.find_one({"name": request.dough_type})
        if dough:
            dough = serialize_mongo_doc(dough)
            total_price += dough["additional_price"]
    
    # Add extras if applicable
    if request.extra_ids:
        for extra_id in request.extra_ids:
            extra = await db.extras.find_one({"id": extra_id})
            if extra:
                extra = serialize_mongo_doc(extra)
                order_item["extras"].append({
                    "id": str(uuid.uuid4()),
                    "name": extra["name"],
                    "price": extra["price"]
                })
                total_price += extra["price"]
    
    order_item["total_price"] = total_price
    
    await db.order_items.insert_one(order_item)
    
    return {"message": "Item added successfully", "item": order_item}

@app.delete("/api/orders/items/{item_id}")
async def remove_item_from_order(item_id: str):
    """Remove an item from an order"""
    result = await db.order_items.delete_one({"id": item_id})
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Item not found")
    
    return {"message": "Item removed successfully"}

@app.get("/api/orders/table/{table_id}")
async def get_order_for_table(table_id: str):
    """Get active order for a table"""
    active_order = await db.orders.find_one({"table_id": table_id, "is_closed": False})
    if not active_order:
        raise HTTPException(status_code=404, detail="No active order found")
    
    active_order = serialize_mongo_doc(active_order)
    
    # Get order items
    items = await db.order_items.find({"order_id": active_order["id"]}).to_list(None)
    items = [serialize_mongo_doc(item) for item in items]
    
    # Calculate total
    total = sum(item["total_price"] for item in items)
    
    return {
        "order": active_order,
        "items": items,
        "total": total
    }

@app.post("/api/orders/{order_id}/send")
async def send_order(order_id: str):
    """Send an order to kitchen/pizzeria"""
    result = await db.orders.update_one(
        {"id": order_id},
        {"$set": {"is_sent": True}}
    )
    
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Order not found")
    
    return {"message": "Order sent successfully"}

@app.get("/api/orders/{order_id}/receipt")
async def get_receipt(order_id: str):
    """Get receipt for an order"""
    order = await db.orders.find_one({"id": order_id})
    if not order:
        raise HTTPException(status_code=404, detail="Order not found")
    
    order = serialize_mongo_doc(order)
    
    # Get table
    table = await db.tables.find_one({"id": order["table_id"]})
    table = serialize_mongo_doc(table)
    
    # Get items
    items = await db.order_items.find({"order_id": order_id}).to_list(None)
    items = [serialize_mongo_doc(item) for item in items]
    
    # Group items by type
    kitchen_items = [item for item in items if item["product_type"] == "kitchen"]
    pizzeria_items = [item for item in items if item["product_type"] == "pizzeria" and item.get("dough_type") != "Senza Glutine"]
    gluten_free_items = [item for item in items if item["product_type"] == "pizzeria" and item.get("dough_type") == "Senza Glutine"]
    
    # Count dough types
    dough_summary = {}
    for item in items:
        if item["product_type"] == "pizzeria" and item.get("dough_type"):
            dough_type = item["dough_type"]
            dough_summary[dough_type] = dough_summary.get(dough_type, 0) + 1
    
    total = sum(item["total_price"] for item in items)
    
    return {
        "order": order,
        "table": table,
        "kitchen_items": kitchen_items,
        "pizzeria_items": pizzeria_items,
        "gluten_free_items": gluten_free_items,
        "dough_summary": dough_summary,
        "total": total
    }

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8001)