import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { productsAPI, ordersAPI, tablesAPI } from '../services/api';
import ProductCard from './ProductCard';
import OrderSidebar from './OrderSidebar';

const MenuView = () => {
  const { tableId } = useParams();
  const navigate = useNavigate();
  const [table, setTable] = useState(null);
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [activeCategory, setActiveCategory] = useState('antipasti');
  const [doughTypes, setDoughTypes] = useState([]);
  const [extras, setExtras] = useState([]);
  const [currentOrder, setCurrentOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchData();
  }, [tableId]);

  useEffect(() => {
    fetchProducts();
  }, [activeCategory]);

  const fetchData = async () => {
    try {
      setLoading(true);
      
      // Fetch table info
      const tableResponse = await tablesAPI.getTable(tableId);
      setTable(tableResponse.data);
      
      // Fetch categories
      const categoriesResponse = await productsAPI.getCategories();
      setCategories(categoriesResponse.data);
      
      // Fetch dough types and extras
      const [doughTypesResponse, extrasResponse] = await Promise.all([
        productsAPI.getDoughTypes(),
        productsAPI.getExtras()
      ]);
      
      setDoughTypes(doughTypesResponse.data);
      setExtras(extrasResponse.data);
      
      // Fetch current order
      await fetchCurrentOrder();
      
      setError(null);
    } catch (err) {
      setError('Errore nel caricamento dei dati');
      console.error('Error fetching data:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchProducts = async () => {
    try {
      const response = await productsAPI.getProducts(activeCategory);
      setProducts(response.data);
    } catch (err) {
      console.error('Error fetching products:', err);
    }
  };

  const fetchCurrentOrder = async () => {
    try {
      const response = await ordersAPI.getOrderForTable(tableId);
      setCurrentOrder(response.data);
    } catch (err) {
      console.error('Error fetching current order:', err);
    }
  };

  const handleAddItem = async (productId, doughType, extraIds) => {
    try {
      await ordersAPI.addItem(tableId, productId, doughType, extraIds);
      await fetchCurrentOrder();
    } catch (err) {
      console.error('Error adding item:', err);
      setError('Errore nell\'aggiunta dell\'articolo');
    }
  };

  const handleRemoveItem = async (itemId) => {
    try {
      await ordersAPI.removeItem(itemId);
      await fetchCurrentOrder();
    } catch (err) {
      console.error('Error removing item:', err);
      setError('Errore nella rimozione dell\'articolo');
    }
  };

  const categoryIcons = {
    antipasti: '🥗',
    pasta: '🍝',
    pizza: '🍕',
    dessert: '🍰'
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-italian-red"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4">
        <div className="flex">
          <div className="text-red-700">{error}</div>
        </div>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div className="lg:col-span-2">
        <div className="card">
          <div className="card-header">
            <h1 className="text-2xl font-bold text-gray-900">
              Menu - Tavolo {table?.number}
            </h1>
            <div className="flex items-center space-x-4 mt-2">
              <span className="text-sm text-gray-600">
                Coperti: {table?.covers}
              </span>
              <button
                onClick={() => navigate('/tables')}
                className="text-sm text-italian-red hover:text-hover-red"
              >
                ← Torna ai tavoli
              </button>
            </div>
          </div>

          {/* Category Navigation */}
          <div className="flex space-x-2 mb-6 overflow-x-auto">
            {categories.map((category) => (
              <button
                key={category}
                onClick={() => setActiveCategory(category)}
                className={`menu-category ${
                  activeCategory === category ? 'active' : ''
                }`}
              >
                <span className="text-lg">{categoryIcons[category]}</span>
                <span className="capitalize">{category}</span>
              </button>
            ))}
          </div>

          {/* Products Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {products.map((product) => (
              <ProductCard
                key={product.id}
                product={product}
                doughTypes={doughTypes}
                extras={extras}
                onAddItem={handleAddItem}
              />
            ))}
          </div>
        </div>
      </div>

      <div className="lg:col-span-1">
        <OrderSidebar
          currentOrder={currentOrder}
          onRemoveItem={handleRemoveItem}
          onGoToSummary={() => navigate(`/order/${tableId}`)}
        />
      </div>
    </div>
  );
};

export default MenuView;