import React from 'react';

const TableCard = ({ table, onClick, onClose }) => {
  const formatPrice = (price) => {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR'
    }).format(price);
  };

  return (
    <div
      className={`table-card cursor-pointer ${
        table.status === 'free' ? 'table-free' : 'table-occupied'
      }`}
      onClick={onClick}
    >
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="text-xl font-bold text-gray-900">
            Tavolo {table.number}
          </h3>
          <div className="flex items-center space-x-2 mt-1">
            <span
              className={`px-2 py-1 text-xs font-medium rounded-full ${
                table.status === 'free'
                  ? 'bg-green-100 text-green-800'
                  : 'bg-red-100 text-red-800'
              }`}
            >
              {table.status === 'free' ? 'Libero' : 'Occupato'}
            </span>
          </div>
        </div>
        <div className="text-right">
          <div className="text-2xl">
            {table.status === 'free' ? '🟢' : '🔴'}
          </div>
        </div>
      </div>

      {table.status === 'occupied' && (
        <div className="space-y-2 mb-4">
          <div className="flex justify-between text-sm">
            <span className="text-gray-600">Coperti:</span>
            <span className="font-medium">{table.covers}</span>
          </div>
          <div className="flex justify-between text-sm">
            <span className="text-gray-600">Articoli:</span>
            <span className="font-medium">{table.items_count || 0}</span>
          </div>
          <div className="flex justify-between text-sm">
            <span className="text-gray-600">Totale:</span>
            <span className="font-medium">{formatPrice(table.total || 0)}</span>
          </div>
        </div>
      )}

      <div className="flex justify-between items-center">
        <div className="text-sm text-gray-500">
          Utilizzi: {table.use_count}
        </div>
        {table.status === 'occupied' && (
          <button
            onClick={(e) => {
              e.stopPropagation();
              onClose();
            }}
            className="px-3 py-1 text-xs bg-italian-red text-white rounded-lg hover:bg-hover-red transition-colors"
          >
            Chiudi
          </button>
        )}
      </div>
    </div>
  );
};

export default TableCard;