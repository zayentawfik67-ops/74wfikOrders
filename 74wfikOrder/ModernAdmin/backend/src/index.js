const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const morgan = require('morgan');
const rateLimit = require('express-rate-limit');
require('dotenv').config();

const { connectDB, sequelize } = require('./config/db');

// Models
const User = require('./models/User');
const Order = require('./models/Order');
const Customer = require('./models/Customer');
const { AuditLog, MessageLog } = require('./models/Logs');

const app = express();

// Middlewares
app.use(helmet());
app.use(cors());
app.use(express.json());
app.use(morgan('dev'));

// Rate Limiting
const limiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutes
  max: 100, // limit each IP to 100 requests per windowMs
});
app.use('/api/', limiter);

// Routes
const authRoutes = require('./routes/authRoutes');
const orderRoutes = require('./routes/orderRoutes');
const customerRoutes = require('./routes/customerRoutes');

app.use('/api/auth', authRoutes);
app.use('/api/orders', orderRoutes);
app.use('/api/customers', customerRoutes);

// Base Route
app.get('/', (req, res) => {
  res.send('KayuSushi Modern Admin API is running...');
});

// Database Sync & Server Start
const PORT = process.env.PORT || 5000;

const startServer = async () => {
  await connectDB();
  
  // Sync associations if any (future-proof)
  Order.belongsTo(Customer, { foreignKey: 'customerId' });
  Customer.hasMany(Order, { foreignKey: 'customerId' });
  
  await sequelize.sync({ alter: true });
  console.log('Database synced.');

  app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
  });
};

startServer();
