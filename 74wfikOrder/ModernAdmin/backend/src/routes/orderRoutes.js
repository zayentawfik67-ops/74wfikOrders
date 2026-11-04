const express = require('express');
const router = express.Router();
const { getStats, getOrders, updateOrderStatus } = require('../controllers/orderController');
const { protect } = require('../middlewares/authMiddleware');

router.get('/stats', protect, getStats);
router.get('/', protect, getOrders);
router.patch('/:id/status', protect, updateOrderStatus);

module.exports = router;
