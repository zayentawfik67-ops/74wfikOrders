const { sequelize } = require('../config/db');
const User = require('../models/User');
const Order = require('../models/Order');
const Customer = require('../models/Customer');
const { AuditLog } = require('../models/Logs');

const seed = async () => {
  try {
    await sequelize.sync({ force: true });
    console.log('Database cleared.');

    // Users
    const admin = await User.create({
      name: 'Admin KayuSushi',
      email: 'admin@74wfikorder.com',
      password: 'password123',
      role: 'Admin',
      isTwoFactorEnabled: true
    });

    const staff = await User.create({
      name: 'Staff Member',
      email: 'staff@74wfikorder.com',
      password: 'password123',
      role: 'Staff'
    });

    // Customers
    const c1 = await Customer.create({
      name: 'Jean Dupont',
      email: 'jean.dupont@email.com',
      phone: '+216 22 333 444'
    });

    const c2 = await Customer.create({
      name: 'Amira Ben Ali',
      email: 'amira.ba@email.com',
      phone: '+216 55 666 777'
    });

    // Orders
    await Order.create({
      orderNumber: 'WF-1001',
      customerId: c1.id,
      total: 45.500,
      status: 'Paid',
      items: [{ name: 'Pizza Royale', qty: 2, price: 15.000 }, { name: 'Coca Cola', qty: 2, price: 2.500 }],
      history: [{ status: 'Pending', updatedAt: new Date(), updatedBy: 'System' }, { status: 'Paid', updatedAt: new Date(), updatedBy: 'System' }]
    });

    await Order.create({
      orderNumber: 'WF-1002',
      customerId: c2.id,
      total: 12.000,
      status: 'Pending',
      items: [{ name: 'Sandwich Chapati', qty: 1, price: 6.500 }, { name: 'Frites', qty: 1, price: 5.500 }],
      history: [{ status: 'Pending', updatedAt: new Date(), updatedBy: 'System' }]
    });

    console.log('Seeding completed successfully.');
    process.exit(0);
  } catch (error) {
    console.error('Seeding failed:', error);
    process.exit(1);
  }
};

seed();
