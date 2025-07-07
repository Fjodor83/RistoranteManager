import requests
import json
import unittest
import time
from typing import Dict, List, Optional

# Get the backend URL from frontend/.env
with open('/app/frontend/.env', 'r') as f:
    for line in f:
        if line.startswith('REACT_APP_BACKEND_URL='):
            BACKEND_URL = line.strip().split('=')[1]
            break

# Ensure the URL doesn't have quotes
BACKEND_URL = BACKEND_URL.strip('"\'')
API_URL = f"{BACKEND_URL}/api"

class RistoranteManagerBackendTest(unittest.TestCase):
    """Test suite for Ristorante Manager Backend API"""
    
    def setUp(self):
        """Setup for each test"""
        # Get all tables to find a free one for testing
        self.tables = self.get_all_tables()
        self.free_table = next((table for table in self.tables if table['status'] == 'free'), None)
        
        # If no free table, close one
        if not self.free_table and self.tables:
            self.close_table(self.tables[0]['id'])
            time.sleep(1)  # Wait for the operation to complete
            self.tables = self.get_all_tables()
            self.free_table = next((table for table in self.tables if table['status'] == 'free'), None)
        
        # Get products for testing
        self.products = self.get_all_products()
        self.pizza_product = next((product for product in self.products if product['category'] == 'pizza'), None)
        self.kitchen_product = next((product for product in self.products if product['type'] == 'kitchen'), None)
        
        # Get dough types and extras
        self.dough_types = self.get_dough_types()
        self.extras = self.get_extras()
        
        # Test data
        self.active_order_id = None
        self.order_item_id = None
    
    def get_all_tables(self) -> List[Dict]:
        """Helper to get all tables"""
        response = requests.get(f"{API_URL}/tables")
        self.assertEqual(response.status_code, 200)
        return response.json()
    
    def get_all_products(self) -> List[Dict]:
        """Helper to get all products"""
        response = requests.get(f"{API_URL}/products")
        self.assertEqual(response.status_code, 200)
        return response.json()
    
    def get_dough_types(self) -> List[Dict]:
        """Helper to get all dough types"""
        response = requests.get(f"{API_URL}/dough-types")
        self.assertEqual(response.status_code, 200)
        return response.json()
    
    def get_extras(self) -> List[Dict]:
        """Helper to get all extras"""
        response = requests.get(f"{API_URL}/extras")
        self.assertEqual(response.status_code, 200)
        return response.json()
    
    def open_table(self, table_id: str, covers: int) -> Dict:
        """Helper to open a table"""
        data = {"table_id": table_id, "covers": covers}
        response = requests.post(f"{API_URL}/tables/open", json=data)
        self.assertEqual(response.status_code, 200)
        return response.json()
    
    def close_table(self, table_id: str) -> Dict:
        """Helper to close a table"""
        response = requests.post(f"{API_URL}/tables/{table_id}/close")
        return response.json()
    
    def add_item_to_order(self, table_id: str, product_id: str, 
                          dough_type: Optional[str] = None, 
                          extra_ids: Optional[List[str]] = None) -> Dict:
        """Helper to add an item to an order"""
        data = {
            "table_id": table_id,
            "product_id": product_id,
            "dough_type": dough_type,
            "extra_ids": extra_ids or []
        }
        print(f"Adding item to order with data: {data}")
        response = requests.post(f"{API_URL}/orders/add-item", json=data)
        print(f"Response status: {response.status_code}")
        if response.status_code != 200:
            print(f"Error response: {response.text}")
            # Let's try to get more information about the error
            try:
                error_data = response.json()
                print(f"Error JSON: {error_data}")
            except:
                print("Could not parse error response as JSON")
        self.assertEqual(response.status_code, 200)
        return response.json()
    
    def test_01_get_tables(self):
        """Test getting all tables"""
        print("\n--- Testing GET /api/tables ---")
        response = requests.get(f"{API_URL}/tables")
        self.assertEqual(response.status_code, 200)
        tables = response.json()
        self.assertIsInstance(tables, list)
        self.assertTrue(len(tables) > 0)
        print(f"✅ Successfully retrieved {len(tables)} tables")
        
        # Check table structure
        table = tables[0]
        self.assertIn('id', table)
        self.assertIn('number', table)
        self.assertIn('status', table)
        self.assertIn('covers', table)
        print("✅ Table structure is correct")
    
    def test_02_get_specific_table(self):
        """Test getting a specific table"""
        print("\n--- Testing GET /api/tables/{table_id} ---")
        if not self.tables:
            self.skipTest("No tables available for testing")
        
        table_id = self.tables[0]['id']
        response = requests.get(f"{API_URL}/tables/{table_id}")
        self.assertEqual(response.status_code, 200)
        table = response.json()
        self.assertEqual(table['id'], table_id)
        print(f"✅ Successfully retrieved table {table_id}")
        
        # Test with invalid table ID
        response = requests.get(f"{API_URL}/tables/invalid-id")
        self.assertEqual(response.status_code, 404)
        print("✅ Correctly returns 404 for invalid table ID")
    
    def test_03_open_and_close_table(self):
        """Test opening and closing a table"""
        print("\n--- Testing POST /api/tables/open and POST /api/tables/{table_id}/close ---")
        if not self.free_table:
            self.skipTest("No free tables available for testing")
        
        # Open table
        table_id = self.free_table['id']
        covers = 4
        result = self.open_table(table_id, covers)
        self.assertIn('message', result)
        self.assertIn('order_id', result)
        print(f"✅ Successfully opened table {table_id} with {covers} covers")
        
        # Verify table status changed
        response = requests.get(f"{API_URL}/tables/{table_id}")
        self.assertEqual(response.status_code, 200)
        table = response.json()
        self.assertEqual(table['status'], 'occupied')
        self.assertEqual(table['covers'], covers)
        print("✅ Table status correctly updated to 'occupied'")
        
        # Close table
        result = self.close_table(table_id)
        self.assertIn('message', result)
        print(f"✅ Successfully closed table {table_id}")
        
        # Verify table status changed back
        response = requests.get(f"{API_URL}/tables/{table_id}")
        self.assertEqual(response.status_code, 200)
        table = response.json()
        self.assertEqual(table['status'], 'free')
        self.assertEqual(table['covers'], 0)
        print("✅ Table status correctly updated to 'free'")
    
    def test_04_get_products(self):
        """Test getting all products and by category"""
        print("\n--- Testing GET /api/products ---")
        # Get all products
        response = requests.get(f"{API_URL}/products")
        self.assertEqual(response.status_code, 200)
        products = response.json()
        self.assertIsInstance(products, list)
        self.assertTrue(len(products) > 0)
        print(f"✅ Successfully retrieved {len(products)} products")
        
        # Get products by category
        categories = ['antipasti', 'pasta', 'pizza', 'dessert']
        for category in categories:
            response = requests.get(f"{API_URL}/products?category={category}")
            self.assertEqual(response.status_code, 200)
            category_products = response.json()
            self.assertIsInstance(category_products, list)
            for product in category_products:
                self.assertEqual(product['category'], category)
            print(f"✅ Successfully retrieved products for category '{category}'")
    
    def test_05_get_categories(self):
        """Test getting product categories"""
        print("\n--- Testing GET /api/products/categories ---")
        response = requests.get(f"{API_URL}/products/categories")
        self.assertEqual(response.status_code, 200)
        categories = response.json()
        self.assertIsInstance(categories, list)
        self.assertTrue(len(categories) > 0)
        expected_categories = ['antipasti', 'pasta', 'pizza', 'dessert']
        for category in expected_categories:
            self.assertIn(category, categories)
        print(f"✅ Successfully retrieved categories: {', '.join(categories)}")
    
    def test_06_get_dough_types(self):
        """Test getting dough types"""
        print("\n--- Testing GET /api/dough-types ---")
        response = requests.get(f"{API_URL}/dough-types")
        self.assertEqual(response.status_code, 200)
        dough_types = response.json()
        self.assertIsInstance(dough_types, list)
        self.assertTrue(len(dough_types) > 0)
        
        # Check dough type structure
        dough_type = dough_types[0]
        self.assertIn('id', dough_type)
        self.assertIn('name', dough_type)
        self.assertIn('additional_price', dough_type)
        print(f"✅ Successfully retrieved {len(dough_types)} dough types")
    
    def test_07_get_extras(self):
        """Test getting extras"""
        print("\n--- Testing GET /api/extras ---")
        response = requests.get(f"{API_URL}/extras")
        self.assertEqual(response.status_code, 200)
        extras = response.json()
        self.assertIsInstance(extras, list)
        self.assertTrue(len(extras) > 0)
        
        # Check extra structure
        extra = extras[0]
        self.assertIn('id', extra)
        self.assertIn('name', extra)
        self.assertIn('price', extra)
        print(f"✅ Successfully retrieved {len(extras)} extras")
    
    def test_08_order_workflow(self):
        """Test complete order workflow"""
        print("\n--- Testing Complete Order Workflow ---")
        if not self.free_table or not self.pizza_product or not self.kitchen_product:
            self.skipTest("Missing required test data")
        
        # Print debug info
        print(f"Free table: {self.free_table}")
        print(f"Kitchen product: {self.kitchen_product}")
        print(f"Pizza product: {self.pizza_product}")
        
        # Step 1: Open a table
        table_id = self.free_table['id']
        covers = 3
        result = self.open_table(table_id, covers)
        order_id = result['order_id']
        print(f"✅ Table opened with order ID: {order_id}")
        
        # Step 2: Add a kitchen item to the order
        kitchen_item = self.add_item_to_order(table_id, self.kitchen_product['id'])
        kitchen_item_id = kitchen_item['item']['id']
        print(f"✅ Added kitchen item: {self.kitchen_product['name']}")
        
        # Step 3: Add a pizza with customizations
        dough_type = self.dough_types[0]['name'] if self.dough_types else None
        extra_ids = [self.extras[0]['id']] if self.extras else []
        
        pizza_item = self.add_item_to_order(
            table_id, 
            self.pizza_product['id'],
            dough_type=dough_type,
            extra_ids=extra_ids
        )
        pizza_item_id = pizza_item['item']['id']
        print(f"✅ Added pizza item: {self.pizza_product['name']} with dough: {dough_type}")
        
        # Step 4: Get order for table
        response = requests.get(f"{API_URL}/orders/table/{table_id}")
        self.assertEqual(response.status_code, 200)
        order_data = response.json()
        self.assertEqual(order_data['order']['id'], order_id)
        self.assertEqual(len(order_data['items']), 2)
        
        # Verify total calculation
        expected_total = (
            self.kitchen_product['price'] + 
            self.pizza_product['price'] + 
            (self.dough_types[0]['additional_price'] if dough_type else 0) +
            (self.extras[0]['price'] if extra_ids else 0)
        )
        self.assertAlmostEqual(order_data['total'], expected_total, places=2)
        print(f"✅ Order retrieved with correct total: {order_data['total']}")
        
        # Step 5: Remove one item
        response = requests.delete(f"{API_URL}/orders/items/{kitchen_item_id}")
        self.assertEqual(response.status_code, 200)
        print(f"✅ Removed kitchen item from order")
        
        # Verify item was removed
        response = requests.get(f"{API_URL}/orders/table/{table_id}")
        self.assertEqual(response.status_code, 200)
        order_data = response.json()
        self.assertEqual(len(order_data['items']), 1)
        
        # Step 6: Send order
        response = requests.post(f"{API_URL}/orders/{order_id}/send")
        self.assertEqual(response.status_code, 200)
        print(f"✅ Order sent successfully")
        
        # Verify order was marked as sent
        response = requests.get(f"{API_URL}/orders/table/{table_id}")
        self.assertEqual(response.status_code, 200)
        order_data = response.json()
        self.assertTrue(order_data['order']['is_sent'])
        
        # Step 7: Get receipt
        response = requests.get(f"{API_URL}/orders/{order_id}/receipt")
        self.assertEqual(response.status_code, 200)
        receipt = response.json()
        self.assertEqual(receipt['order']['id'], order_id)
        self.assertEqual(receipt['table']['id'], table_id)
        
        # Check receipt structure
        self.assertIn('kitchen_items', receipt)
        self.assertIn('pizzeria_items', receipt)
        self.assertIn('gluten_free_items', receipt)
        self.assertIn('dough_summary', receipt)
        self.assertIn('total', receipt)
        print(f"✅ Receipt generated successfully with total: {receipt['total']}")
        
        # Step 8: Close table
        result = self.close_table(table_id)
        print(f"✅ Table closed successfully")
        
        # Verify table is free again
        response = requests.get(f"{API_URL}/tables/{table_id}")
        self.assertEqual(response.status_code, 200)
        table = response.json()
        self.assertEqual(table['status'], 'free')
    
    def test_09_error_handling(self):
        """Test error handling for various scenarios"""
        print("\n--- Testing Error Handling ---")
        
        # Test invalid table ID
        response = requests.get(f"{API_URL}/tables/invalid-id")
        self.assertEqual(response.status_code, 404)
        print("✅ Correctly handles invalid table ID")
        
        # Test invalid product ID in add item
        if self.free_table:
            # First open a table
            table_id = self.free_table['id']
            result = self.open_table(table_id, 2)
            
            # Try to add invalid product
            data = {"table_id": table_id, "product_id": "invalid-id", "extra_ids": []}
            response = requests.post(f"{API_URL}/orders/add-item", json=data)
            self.assertEqual(response.status_code, 404)
            print("✅ Correctly handles invalid product ID")
            
            # Close the table
            self.close_table(table_id)
        
        # Test invalid order ID
        response = requests.get(f"{API_URL}/orders/invalid-id/receipt")
        self.assertEqual(response.status_code, 404)
        print("✅ Correctly handles invalid order ID")
        
        # Test invalid item ID
        response = requests.delete(f"{API_URL}/orders/items/invalid-id")
        self.assertEqual(response.status_code, 404)
        print("✅ Correctly handles invalid item ID")


if __name__ == "__main__":
    print(f"Testing Ristorante Manager Backend API at: {API_URL}")
    unittest.main(verbosity=2)