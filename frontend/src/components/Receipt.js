import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ordersAPI, tablesAPI } from '../services/api';

const Receipt = () => {
  const { orderId } = useParams();
  const navigate = useNavigate();
  const [receiptData, setReceiptData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [closing, setClosing] = useState(false);

  useEffect(() => {
    fetchReceipt();
  }, [orderId]);

  const fetchReceipt = async () => {
    try {
      setLoading(true);
      const response = await ordersAPI.getReceipt(orderId);
      setReceiptData(response.data);
      setError(null);
    } catch (err) {
      setError('Errore nel caricamento della ricevuta');
      console.error('Error fetching receipt:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCloseTable = async () => {
    try {
      setClosing(true);
      await tablesAPI.closeTable(receiptData.table.id);
      navigate('/tables');
    } catch (err) {
      console.error('Error closing table:', err);
      setError('Errore nella chiusura del tavolo');
    } finally {
      setClosing(false);
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

  if (!receiptData) {
    return (
      <div className="text-center">
        <div className="text-4xl mb-4">📄</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">Ricevuta non trovata</h2>
        <button
          onClick={() => navigate('/tables')}
          className="btn-primary"
        >
          Torna ai Tavoli
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="card">
        <div className="text-center mb-6">
          <h1 className="text-3xl font-bold text-italian-red font-italiana">
            🍝 Ristorante Manager
          </h1>
          <div className="text-gray-600 mt-2">
            Ricevuta Ordine #{orderId.slice(0, 8)}
          </div>
        </div>

        <div className="receipt-section">
          <div className="flex justify-between items-center">
            <div>
              <div className="font-bold text-lg">Tavolo {receiptData.table.number}</div>
              <div className="text-sm text-gray-600">
                Coperti: {receiptData.table.covers}
              </div>
            </div>
            <div className="text-right text-sm text-gray-600">
              {new Date(receiptData.order.created_at).toLocaleString('it-IT')}
            </div>
          </div>
        </div>

        {/* Kitchen Items */}
        {receiptData.kitchen_items.length > 0 && (
          <div className="receipt-section">
            <h3 className="text-lg font-semibold text-gray-900 mb-3 flex items-center">
              👨‍🍳 Cucina
            </h3>
            <div className="space-y-2">
              {receiptData.kitchen_items.map((item) => (
                <div key={item.id} className="flex justify-between items-start">
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
        {receiptData.pizzeria_items.length > 0 && (
          <div className="receipt-section">
            <h3 className="text-lg font-semibold text-gray-900 mb-3 flex items-center">
              🍕 Pizzeria
            </h3>
            <div className="space-y-2">
              {receiptData.pizzeria_items.map((item) => (
                <div key={item.id} className="flex justify-between items-start">
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
        {receiptData.gluten_free_items.length > 0 && (
          <div className="receipt-section">
            <h3 className="text-lg font-semibold text-gray-900 mb-3 flex items-center">
              🌾 Senza Glutine
            </h3>
            <div className="space-y-2">
              {receiptData.gluten_free_items.map((item) => (
                <div key={item.id} className="flex justify-between items-start">
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

        {/* Dough Summary */}
        {Object.keys(receiptData.dough_summary).length > 0 && (
          <div className="receipt-section">
            <h4 className="font-semibold text-gray-900 mb-2">Riepilogo Impasti:</h4>
            <div className="text-sm text-gray-600 space-y-1">
              {Object.entries(receiptData.dough_summary).map(([dough, count]) => (
                <div key={dough} className="flex justify-between">
                  <span>{dough}:</span>
                  <span>{count} pz</span>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Total */}
        <div className="border-t pt-4">
          <div className="flex justify-between items-center mb-6">
            <span className="text-2xl font-bold text-gray-900">TOTALE:</span>
            <span className="text-2xl font-bold text-italian-red">
              {formatPrice(receiptData.total)}
            </span>
          </div>
        </div>

        {/* Actions */}
        <div className="flex space-x-4">
          <button
            onClick={() => navigate('/tables')}
            className="flex-1 btn-outline"
          >
            Torna ai Tavoli
          </button>
          <button
            onClick={handleCloseTable}
            disabled={closing}
            className="flex-1 btn-primary"
          >
            {closing ? 'Chiusura...' : 'Chiudi Tavolo'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default Receipt;