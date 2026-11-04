const jwt = require('jsonwebtoken');
const User = require('../models/User');
const { AuditLog } = require('../models/Logs');

const generateToken = (id) => {
  return jwt.sign({ id }, process.env.JWT_SECRET, { expiresIn: '1h' });
};

const generateRefreshToken = (id) => {
  return jwt.sign({ id }, process.env.JWT_REFRESH_SECRET, { expiresIn: '7d' });
};

exports.login = async (req, res) => {
  try {
    const { email, password, twoFactorCode } = req.body;

    const user = await User.findOne({ where: { email } });
    if (!user || !(await user.comparePassword(password))) {
      return res.status(401).json({ message: 'Invalid credentials' });
    }

    // Optional 2FA Check
    if (user.isTwoFactorEnabled) {
      if (!twoFactorCode) {
        return res.status(200).json({ require2FA: true, userId: user.id });
      }
      // Demo logic: accept any 6-digit code for now, or implement speakeasy
      if (twoFactorCode !== '123456') {
        return res.status(401).json({ message: 'Invalid 2FA code' });
      }
    }

    const token = generateToken(user.id);
    const refreshToken = generateRefreshToken(user.id);

    // Audit Log
    await AuditLog.create({
      adminId: user.id,
      adminName: user.name,
      action: 'LOGIN',
      details: `User ${user.email} logged in`,
      ipAddress: req.ip,
    });

    res.json({
      token,
      refreshToken,
      user: {
        id: user.id,
        name: user.name,
        email: user.email,
        role: user.role,
      },
    });
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
};
