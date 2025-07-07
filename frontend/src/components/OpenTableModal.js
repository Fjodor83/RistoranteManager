import React, { useState } from 'react';

const OpenTableModal = ({ table, onClose, onConfirm }) => {
  const [covers, setCovers] = useState(1);

  const handleSubmit = (e) => {
    e.preventDefault();
    onConfirm(table.id, covers);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl p-6 max-w-md w-full mx-4">
        <h2 className="text-xl font-bold text-gray-900 mb-4">
          Apri Tavolo {table.number}
        </h2>
        
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Numero di coperti
            </label>
            <input
              type="number"
              min="1"
              max="20"
              value={covers}
              onChange={(e) => setCovers(parseInt(e.target.value))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-italian-red"
              required
            />
          </div>
          
          <div className="flex space-x-4">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-4 py-2 text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-lg transition-colors"
            >
              Annulla
            </button>
            <button
              type="submit"
              className="flex-1 btn-primary"
            >
              Apri Tavolo
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default OpenTableModal;