import React, { useState } from 'react';

const ProductCard = ({ product, doughTypes, extras, onAddItem }) => {
  const [selectedDoughType, setSelectedDoughType] = useState('');
  const [selectedExtras, setSelectedExtras] = useState([]);
  const [showCustomization, setShowCustomization] = useState(false);

  const formatPrice = (price) => {
    return new Intl.NumberFormat('it-IT', {
      style: 'currency',
      currency: 'EUR'
    }).format(price);
  };

  const handleAddToOrder = () => {
    if (product.is_customizable && !showCustomization) {
      setShowCustomization(true);
      return;
    }

    const extraIds = selectedExtras.map(extra => extra.id);
    onAddItem(product.id, selectedDoughType, extraIds);

    // Reset customization
    setSelectedDoughType('');
    setSelectedExtras([]);
    setShowCustomization(false);
  };

  const handleExtraToggle = (extra) => {
    setSelectedExtras(prev => {
      const exists = prev.find(e => e.id === extra.id);
      if (exists) {
        return prev.filter(e => e.id !== extra.id);
      } else {
        return [...prev, extra];
      }
    });
  };

  const calculateTotalPrice = () => {
    let total = product.price;
    
    if (selectedDoughType) {
      const dough = doughTypes.find(d => d.name === selectedDoughType);
      if (dough) {
        total += dough.additional_price;
      }
    }
    
    selectedExtras.forEach(extra => {
      total += extra.price;
    });
    
    return total;
  };

  return (
    <div className="product-card">
      <div className="flex justify-between items-start mb-3">
        <div>
          <h3 className="font-semibold text-gray-900">{product.name}</h3>
          <p className="text-sm text-gray-600">{product.category}</p>
        </div>
        <div className="text-right">
          <div className="font-bold text-italian-red">
            {formatPrice(showCustomization ? calculateTotalPrice() : product.price)}
          </div>
          {product.is_customizable && (
            <div className="text-xs text-gray-500 mt-1">
              Personalizzabile
            </div>
          )}
        </div>
      </div>

      {showCustomization && product.is_customizable && (
        <div className="space-y-4 mb-4 p-4 bg-gray-50 rounded-lg">
          {/* Dough Type Selection */}
          {product.category === 'pizza' && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Tipo di impasto
              </label>
              <select
                value={selectedDoughType}
                onChange={(e) => setSelectedDoughType(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-italian-red"
              >
                <option value="">Seleziona impasto</option>
                {doughTypes.map((dough) => (
                  <option key={dough.id} value={dough.name}>
                    {dough.name} 
                    {dough.additional_price > 0 && ` (+${formatPrice(dough.additional_price)})`}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Extras Selection */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Aggiunte
            </label>
            <div className="space-y-2">
              {extras.map((extra) => (
                <label key={extra.id} className="flex items-center space-x-2">
                  <input
                    type="checkbox"
                    checked={selectedExtras.some(e => e.id === extra.id)}
                    onChange={() => handleExtraToggle(extra)}
                    className="rounded border-gray-300 text-italian-red focus:ring-italian-red"
                  />
                  <span className="text-sm text-gray-700">
                    {extra.name} (+{formatPrice(extra.price)})
                  </span>
                </label>
              ))}
            </div>
          </div>

          <div className="flex space-x-2">
            <button
              onClick={() => setShowCustomization(false)}
              className="flex-1 px-3 py-2 text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-lg transition-colors"
            >
              Annulla
            </button>
            <button
              onClick={handleAddToOrder}
              className="flex-1 btn-primary text-sm"
            >
              Aggiungi ({formatPrice(calculateTotalPrice())})
            </button>
          </div>
        </div>
      )}

      {!showCustomization && (
        <button
          onClick={handleAddToOrder}
          className="w-full btn-primary text-sm"
        >
          {product.is_customizable ? 'Personalizza' : 'Aggiungi'}
        </button>
      )}
    </div>
  );
};

export default ProductCard;