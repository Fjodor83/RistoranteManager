import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header';
import TablesView from './components/TablesView';
import MenuView from './components/MenuView';
import OrderSummary from './components/OrderSummary';
import Receipt from './components/Receipt';

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-50">
        <Header />
        <main className="container mx-auto px-4 py-8">
          <Routes>
            <Route path="/" element={<TablesView />} />
            <Route path="/tables" element={<TablesView />} />
            <Route path="/menu/:tableId" element={<MenuView />} />
            <Route path="/order/:tableId" element={<OrderSummary />} />
            <Route path="/receipt/:orderId" element={<Receipt />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;