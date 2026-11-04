const Order = require('../models/Order');
const Customer = require('../models/Customer');
const { AuditLog } = require('../models/Logs');
const { Sequelize } = require('sequelize');

exports.getStats = async (req, res) => {
  try {
    const totalOrders = await Order.count();
    const pendingOrders = await Order.count({ where: { status: 'Pending' } });
    const revenue = await Order.sum('total') || 0;
    
    // Last 7 days trend (Demo mock data logic)
    const recentActivity = await AuditLog.findAll({ limit: 5, order: [['createdAt', 'DESC']] });

    res.json({
      totalOrders,
      pendingOrders,
      revenue,
      recentActivity
    });
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
};

exports.getOrders = async (req, res) => {
  try {
    const { page = 1, limit = 10, status } = req.query;
    const offset = (page - 1) * limit;
    
    const where = {};
    if (status) where.status = status;

    const { count, rows } = await Order.findAndCountAll({
      where,
      limit: parseInt(limit),
      offset,
      order: [['createdAt', 'DESC']],
      include: [{ model: Customer, attributes: ['name', 'email'] }]
    });

    res.json({
      orders: rows,
      totalPages: Math.ceil(count / limit),
      currentPage: page,
      totalOrders: count
    });
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
};

exports.updateOrderStatus = async (req, res) => {
  try {
    const { status } = req.body;
    const order = await Order.findByPk(req.params.id);
    if (!order) return res.status(404).json({ message: 'Order not found' });

    const oldStatus = order.status;
    order.status = status;
    
    const history = order.history || [];
    history.push({
      status,
      updatedAt: new Date(),
      updatedBy: req.user.name
    });
    order.history = history;
    
    await order.save();

    await AuditLog.create({
      adminId: req.user.id,
      adminName: req.user.name,
      action: 'UPDATE_ORDER_STATUS',
      details: `Order ${order.orderNumber} changed from ${oldStatus} to ${status}`,
      ipAddress: req.ip
    });

    res.json(order);
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
};
