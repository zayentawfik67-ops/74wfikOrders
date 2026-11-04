const { DataTypes } = require('sequelize');
const { sequelize } = require('../config/db');

const Customer = sequelize.define('Customer', {
  id: {
    type: DataTypes.UUID,
    defaultValue: DataTypes.UUIDV4,
    primaryKey: true,
  },
  name: {
    type: DataTypes.STRING,
    allowNull: false,
  },
  email: {
    type: DataTypes.STRING,
    unique: true,
    allowNull: false,
  },
  phone: {
    type: DataTypes.STRING,
    allowNull: false,
  },
  status: {
    type: DataTypes.ENUM('Verified', 'Pending_Verification'),
    defaultValue: 'Verified',
  },
  pendingEmail: {
    type: DataTypes.STRING,
    allowNull: true,
  },
  pendingPhone: {
    type: DataTypes.STRING,
    allowNull: true,
  },
  verificationOTP: {
    type: DataTypes.STRING,
    allowNull: true,
  },
});

module.exports = Customer;
