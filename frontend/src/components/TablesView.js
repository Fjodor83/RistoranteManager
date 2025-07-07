import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { tablesAPI } from '../services/api';
import TableCard from './TableCard';
import OpenTableModal from './OpenTableModal';

const TablesView = () => {
  const [tables, setTables] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedTable, setSelectedTable] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    fetchTables();
  }, []);

  const fetchTables = async () => {
    try {
      setLoading(true);
      const response = await tablesAPI.getTables();
      setTables(response.data);
      setError(null);
    } catch (err) {
      setError('Errore nel caricamento dei tavoli');
      console.error('Error fetching tables:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleTableClick = (table) => {
    if (table.status === 'free') {
      setSelectedTable(table);
      setIsModalOpen(true);
    } else {
      navigate(`/menu/${table.id}`);
    }
  };

  const handleOpenTable = async (tableId, covers) => {
    try {
      await tablesAPI.openTable(tableId, covers);
      setIsModalOpen(false);
      setSelectedTable(null);
      fetchTables();
    } catch (err) {
      console.error('Error opening table:', err);
      setError('Errore nell\'apertura del tavolo');
    }
  };

  const handleCloseTable = async (tableId) => {
    try {
      await tablesAPI.closeTable(tableId);
      fetchTables();
    } catch (err) {
      console.error('Error closing table:', err);
      setError('Errore nella chiusura del tavolo');
    }
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
    <div className="space-y-6">
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Gestione Tavoli</h1>
        <p className="text-gray-600">Clicca su un tavolo per aprirlo o gestire l'ordine</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {tables.map((table) => (
          <TableCard
            key={table.id}
            table={table}
            onClick={() => handleTableClick(table)}
            onClose={() => handleCloseTable(table.id)}
          />
        ))}
      </div>

      {isModalOpen && selectedTable && (
        <OpenTableModal
          table={selectedTable}
          onClose={() => setIsModalOpen(false)}
          onConfirm={handleOpenTable}
        />
      )}
    </div>
  );
};

export default TablesView;