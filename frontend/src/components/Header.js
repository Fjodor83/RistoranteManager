import React from 'react';
import { Link, useLocation } from 'react-router-dom';

const Header = () => {
  const location = useLocation();
  
  return (
    <header className="bg-italian-red shadow-lg">
      <div className="container mx-auto px-4 py-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <Link to="/" className="text-2xl font-bold text-white font-italiana">
              🍝 Ristorante Manager
            </Link>
          </div>
          <nav className="flex space-x-6">
            <Link 
              to="/tables" 
              className={`px-4 py-2 rounded-lg font-medium transition-colors duration-200 ${
                location.pathname === '/tables' || location.pathname === '/' 
                  ? 'bg-white text-italian-red' 
                  : 'text-white hover:bg-red-600'
              }`}
            >
              Tavoli
            </Link>
          </nav>
        </div>
      </div>
    </header>
  );
};

export default Header;