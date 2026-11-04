const express = require('express');
const router = express.Router();
const { getCustomers, requestChange, verifyChange } = require('../controllers/customerController');
const { protect } = require('../middlewares/authMiddleware');

router.get('/', protect, getCustomers);
router.post('/:id/request-change', protect, requestChange);
router.post('/:id/verify-change', protect, verifyChange);

module.exports = router;
