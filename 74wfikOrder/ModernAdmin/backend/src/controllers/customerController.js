const Customer = require('../models/Customer');
const { AuditLog, MessageLog } = require('../models/Logs');

exports.getCustomers = async (req, res) => {
  try {
    const customers = await Customer.findAll();
    res.json(customers);
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
};

exports.requestChange = async (req, res) => {
  try {
    const { email, phone } = req.body;
    const customer = await Customer.findByPk(req.params.id);
    if (!customer) return res.status(404).json({ message: 'Customer not found' });

    const otp = Math.floor(100000 + Math.random() * 900000).toString();
    
    customer.pendingEmail = email || customer.email;
    customer.pendingPhone = phone || customer.phone;
    customer.verificationOTP = otp;
    customer.status = 'Pending_Verification';
    
    await customer.save();

    // Log the action
    await AuditLog.create({
      adminId: req.user.id,
      adminName: req.user.name,
      action: 'CUSTOMER_CHANGE_REQUEST',
      details: `Request to update customer ${customer.name} (OTP: ${otp})`,
      ipAddress: req.ip
    });

    // Simulate sending message
    const messageContent = `Votre code de vérification pour la mise à jour de votre compte est : ${otp}`;
    await MessageLog.create({
      customerId: customer.id,
      customerName: customer.name,
      templateName: 'VERIFICATION_CODE',
      content: messageContent,
      type: 'Email'
    });

    res.json({ message: 'Verification request sent to customer.', otp }); // Sending OTP in response for demo purposes
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
};

exports.verifyChange = async (req, res) => {
  try {
    const { otp } = req.body;
    const customer = await Customer.findByPk(req.params.id);
    if (!customer) return res.status(404).json({ message: 'Customer not found' });

    if (customer.verificationOTP !== otp) {
      return res.status(400).json({ message: 'Invalid OTP' });
    }

    customer.email = customer.pendingEmail;
    customer.phone = customer.pendingPhone;
    customer.pendingEmail = null;
    customer.pendingPhone = null;
    customer.verificationOTP = null;
    customer.status = 'Verified';

    await customer.save();

    res.json({ message: 'Customer info updated successfully.', customer });
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
};
