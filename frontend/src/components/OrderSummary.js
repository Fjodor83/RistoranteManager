import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ordersAPI, tablesAPI } from '../services/api';

const OrderSummary = () => {
  const { tableId } = useParams();
  const navigate = useNavigate();
  const [table, setTable] = useState(null);
  const [currentOrder, setCurrentOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [sending, setSending] = useState(false);

  useEffect(() => {
    fetchData();
  }, [tableId]);

  const fetchData = async () => {
    try {
      setLoading(true);
      
      // Fetch table info
      const tableResponse = await tablesAPI.getTable(tableId);
      setTable(tableResponse.data);
      
      // Fetch current order
      const orderResponse = await ordersAPI.getOrderForTable(tableId);
      setCurrentOrder(orderResponse.data);
      
      setError(null);
    } catch (err) {
      setError('Errore nel caricamento dei dati');
      console.error('Error fetching data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSendOrder = async () => {
    try {
      setSending(true);
      await ordersAPI.sendOrder(currentOrder.order.id);
      navigate(`/receipt/${currentOrder.order.id}`);
    } catch (err) {
      console.error('Error sending order:', err);
      setError('Errore nell\'invio dell\'ordine');
    } finally {
      setSending(false);
    }
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR'
    }).format(price);
  };

  const formatCustomizations = (item) => {
    const customizations = [];
    
    if (item.dough_type) {
      customizations.push(item.dough_type);
    }
    
    if (item.extras && item.extras.length > 0) {
      item.extras.forEach(extra => {
        customizations.push(extra.name);
      });
    }
    
    return customizations;
  };

  const groupItemsByType = (items) => {
    const groups = {
      kitchen: [],
      pizzeria: [],
      glutenFree: []
    };

    items.forEach(item => {
      if (item.product_type === 'kitchen') {
        groups.kitchen.push(item);
      } else if (item.product_type === 'pizzeria') {
        if (item.dough_type === 'Senza Glutine') {
          groups.glutenFree.push(item);
        } else {
          groups.pizzeria.push(item);
        }
      }
    });

    return groups;
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

  if (!currentOrder || !currentOrder.items || currentOrder.items.length === 0) {
    return (
      <div className="card text-center">
        <div className="text-4xl mb-4">🍽️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">Ordine Vuoto</h2>
        <p className="text-gray-600 mb-4">Non ci sono articoli nell'ordine</p>
        <button
          onClick={() => navigate(`/menu/${tableId}`)}
          className="btn-primary"
        >
          Torna al Menu
        </button>
      </div>
    );
  }

  const groupedItems = groupItemsByType(currentOrder.items);

  return (
    <div className="space-y-6">
      <div className="card">
        <div className="card-header">
          <h1 className="text-2xl font-bold text-gray-900">
            Riepilogo Ordine - Tavolo {table?.number}
          </h1>
          <div className="flex items-center space-x-4 mt-2">
            <span className="text-sm text-gray-600">
              Coperti: {table?.covers}
            </span>
            <button
              onClick={() => navigate(`/menu/${tableId}`)}
              className="text-sm text-italian-red hover:text-hover-red"
            >
              ← Torna al Menu
            </button>
          </div>
        </div>

        {/* Kitchen Items */}
        {groupedItems.kitchen.length > 0 && (
          <div className="receipt-section">
            <h3 className="text-lg font-semibold text-gray-900 mb-3 flex items-center">
              👨‍🍳 Cucina
            </h3>
            <div className="space-y-2">
              {groupedItems.kitchen.map((item) => (
                <div key={item.id} className="flex justify-between items-start p-3 bg-gray-50 rounded-lg">
                  <div className="flex-1">
                    <div className="font-medium text-gray-900">{item.name}</div>
                    {formatCustomizations(item).length > 0 && (
                      <div className="text-sm text-gray-500 mt-1">
                        {formatCustomizations(item).join(', ')}
                      </div>
                    )}
                  </div>
                  <div className="text-right">
                    <div className="font-medium text-italian-red">
                      {formatPrice(item.total_price)}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Pizzeria Items */}
        {groupedItems.pizzeria.length > 0 && (
          <div className="receipt-section">
            <h3 className="text-lg font-semibold text-gray-900 mb-3 flex items-center">
              🍕 Pizzeria
            </h3>
            <div className="space-y-2">
              {groupedItems.pizzeria.map((item) => (
                <div key={item.id} className="flex justify-between items-start p-3 bg-gray-50 rounded-lg">
                  <div className="flex-1">
                    <div className="font-medium text-gray-900">{item.name}</div>
                    {formatCustomizations(item).length > 0 && (
                      <div className="text-sm text-gray-500 mt-1">
                        {formatCustomizations(item).join(', ')}
                      </div>
                    )}
                  </div>
                  <div className="text-right">
                    <div className="font-medium text-italian-red">
                      {formatPrice(item.total_price)}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Gluten Free Items */}
        {groupedItems.glutenFree.length > 0 && (
          <div className="receipt-section">
            <h3 className="text-lg font-semibold text-gray-900 mb-3 flex items-center">
              🌾 Senza Glutine
            </h3>
            <div className="space-y-2">
              {groupedItems.glutenFree.map((item) => (
                <div key={item.id} className="flex justify-between items-start p-3 bg-gray-50 rounded-lg">
                  <div className="flex-1">
                    <div className="font-medium text-gray-900">{item.name}</div>
                    {formatCustomizations(item).length > 0 && (
                      <div className="text-sm text-gray-500 mt-1">
                        {formatCustomizations(item).join(', ')}
                      </div>
                    )}
                  </div>
                  <div className="text-right">
                    <div className="font-medium text-italian-red">
                      {formatPrice(item.total_price)}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Total */}
        <div className="border-t pt-4">
          <div className="flex justify-between items-center mb-6">
            <span className="text-xl font-bold text-gray-900">Totale:</span>
            <span className="text-xl font-bold text-italian-red">
              {formatPrice(currentOrder.total)}
            </span>
          </div>
          
          <div className="flex space-x-4">
            <button
              onClick={() => navigate(`/menu/${tableId}`)}
              className="flex-1 btn-outline"
            >
              Aggiungi Articoli
            </button>
            <button
              onClick={handleSendOrder}
              disabled={sending}
              className="flex-1 btn-primary"
            >
              {sending ? 'Invio in corso...' : 'Invia Ordine'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderSummary;