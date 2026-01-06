const User = require('../../models/User');

// Webhook handler
export default async function handler(req, res) {
  // CORS headers
  res.setHeader('Access-Control-Allow-Credentials', true);
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'GET,OPTIONS,PATCH,DELETE,POST,PUT');
  res.setHeader(
    'Access-Control-Allow-Headers',
    'X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Content-Type, Date, X-Api-Version, Authorization'
  );

  // Handle OPTIONS request
  if (req.method === 'OPTIONS') {
    return res.status(200).end();
  }

  // Only allow POST requests
  if (req.method !== 'POST') {
    return res.status(405).json({ error: 'Method not allowed' });
  }

  try {
    // Verify webhook authentication
    const authHeader = req.headers.authorization;
    if (!authHeader || authHeader !== `Bearer ${process.env.REVENUECAT_WEBHOOK_SECRET}`) {
      console.error('Unauthorized webhook request');
      return res.status(401).json({ error: 'Unauthorized' });
    }

    const event = req.body;
    console.log('RevenueCat webhook event:', event);

    switch (event.type) {
      case 'SUBSCRIPTION_CANCELLED':
      case 'SUBSCRIPTION_EXPIRED':
        await handleSubscriptionEnd(event);
        break;

      case 'SUBSCRIPTION_RENEWED':
      case 'SUBSCRIPTION_REACTIVATED':
        await handleSubscriptionRenewal(event);
        break;

      default:
        console.log('Unhandled event type:', event.type);
    }

    res.status(200).json({ success: true });
  } catch (error) {
    console.error('Webhook handler error:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
}

async function handleSubscriptionEnd(event) {
  try {
    const user = await User.findOne({ rcUserId: event.app_user_id });
    if (!user) {
      console.error(`User not found: ${event.app_user_id}`);
      return;
    }

    user.isMembership = false;
    user.membershipType = 'none';
    user.membershipExpireDate = null;
    user.lastWebhookEvent = {
      type: event.type,
      timestamp: new Date(event.event_timestamp),
      originalEvent: event
    };

    await user.save();
    console.log(`Subscription ended for user: ${event.app_user_id}`);
  } catch (error) {
    console.error('Error handling subscription end:', error);
    throw error;
  }
}

async function handleSubscriptionRenewal(event) {
  try {
    const user = await User.findOne({ rcUserId: event.app_user_id });
    if (!user) {
      console.error(`User not found: ${event.app_user_id}`);
      return;
    }

    // Calculate subscription expiration
    const expireDate = new Date(event.event_timestamp);
    const isMonthly = event.product_id.includes('monthly');
    
    if (isMonthly) {
      expireDate.setMonth(expireDate.getMonth() + 1);
    } else {
      expireDate.setFullYear(expireDate.getFullYear() + 1);
    }

    user.isMembership = true;
    user.membershipType = isMonthly ? 'monthly' : 'yearly';
    user.membershipExpireDate = expireDate;
    user.lastWebhookEvent = {
      type: event.type,
      timestamp: new Date(event.event_timestamp),
      originalEvent: event
    };

    await user.save();
    console.log(`Subscription renewed for user: ${event.app_user_id}`);
  } catch (error) {
    console.error('Error handling subscription renewal:', error);
    throw error;
  }
}