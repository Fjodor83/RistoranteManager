import React from 'react';

const OrderSidebar = ({ currentOrder, onRemoveItem, onGoToSummary }) => {
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

  if (!currentOrder || !currentOrder.items || currentOrder.items.length === 0) {
    return (
      <div className="card">
        <h2 className="text-xl font-bold text-gray-900 mb-4">Ordine Corrente</h2>
        <div className="text-center text-gray-500 py-8">
          <div className="text-4xl mb-2">🍽️</div>
          <p>Nessun articolo nell'ordine</p>
        </div>
      </div>
    );
  }

  return (
    <div className="card">
      <h2 className="text-xl font-bold text-gray-900 mb-4">
        Ordine Corrente
      </h2>
      
      <div className="space-y-3 mb-6">
        {currentOrder.items.map((item) => (
          <div key={item.id} className="order-item">
            <div className="flex-1">
              <div className="font-medium text-gray-900">{item.name}</div>
              {formatCustomizations(item).length > 0 && (
                <div className="text-xs text-gray-500 mt-1">
                  {formatCustomizations(item).join(', ')}
                </div>
              )}
              <div className="text-sm font-medium text-italian-red mt-1">
                {formatPrice(item.total_price)}
              </div>
            </div>
            <button
              onClick={() => onRemoveItem(item.id)}
              className="text-red-600 hover:text-red-800 ml-3"
            >
              ✕
            </button>
          </div>
        ))}
      </div>

      <div className="border-t pt-4">
        <div className="flex justify-between items-center mb-4">
          <span className="text-lg font-bold text-gray-900">Totale:</span>
          <span className="text-lg font-bold text-italian-red">
            {formatPrice(currentOrder.total)}
          </span>
        </div>
        
        <button
          onClick={onGoToSummary}
          className="w-full btn-primary"
        >
          Riepilogo Ordine
        </button>
      </div>
    </div>
  );
};

export default OrderSidebar;