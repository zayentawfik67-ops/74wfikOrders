const { DataTypes } = require('sequelize');
const { sequelize } = require('../config/db');

const Order = sequelize.define('Order', {
  id: {
    type: DataTypes.UUID,
    defaultValue: DataTypes.UUIDV4,
    primaryKey: true,
  },
  orderNumber: {
    type: DataTypes.STRING,
    allowNull: false,
    unique: true,
  },
  customerId: {
    type: DataTypes.UUID,
    allowNull: false,
  },
  total: {
    type: DataTypes.DECIMAL(10, 2),
    allowNull: false,
  },
  status: {
    type: DataTypes.ENUM('Pending', 'Paid', 'Shipped', 'Cancelled'),
    defaultValue: 'Pending',
  },
  items: {
    type: DataTypes.JSON, // SQLite supports JSON via stringification in Sequelize
    allowNull: false,
  },
  history: {
    type: DataTypes.JSON,
    defaultValue: [],
  },
});

module.exports = Order;
