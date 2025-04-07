const { https } = require('firebase-functions/v2');
const admin = require("firebase-admin");
const { google } = require("googleapis");

const serviceAccount = require("./play-billing-service.json");

admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    databaseURL: "https://sure-chronicle-8ffce-default-rtdb.firebaseio.com"
});

const playDeveloperApi = google.androidpublisher({
    version: "v3",
    auth: new google.auth.GoogleAuth({
        credentials: serviceAccount,
        scopes: ["https://www.googleapis.com/auth/androidpublisher"],
    }),
});

exports.verifyPurchase = https.onCall(async (data, context) => {
    console.log("✅ Firebase received full data.");

    try {
        const requestData = data.data || data;
        const { packageName, productId, purchaseToken } = requestData;

        if (!packageName || !productId || !purchaseToken) {
            throw new https.HttpsError("invalid-argument", "Missing parameters.");
        }

        console.log(`📦 packageName: ${packageName}, 📜 productId: ${productId}, 🔑 purchaseToken: ${purchaseToken}`);

        let purchaseData;
        try {
            const response = await playDeveloperApi.purchases.products.get({
                packageName: packageName,
                productId: productId,
                token: purchaseToken,
            });

            console.log("✅ Google Play API Response received.");
            purchaseData = response.data;
        } catch (apiError) {
            console.error("🔥 Google Play API Request Failed:", apiError.toString());

            if (apiError.response) {
                console.error("🔥 Google API Error Status:", apiError.response.status);
                console.error("🔥 Google API Error Data:", JSON.stringify(apiError.response.data, null, 2));
            }

            throw new https.HttpsError("internal", `Google Play API 오류: ${apiError.message}`);
        }

        if (purchaseData.purchaseState === 0) {
            const userId = context.auth ? context.auth.uid : "anonymous";
            await admin.database().ref(`purchases/${userId}/${productId}`).set({
                purchaseToken: purchaseToken,
                purchaseTime: purchaseData.purchaseTimeMillis,
                acknowledged: purchaseData.acknowledgementState === 1
            });

            console.log("✅ Purchase verified and saved.");
            return { success: true, message: "Purchase is valid and saved to database." };
        } else {
            return { success: false, message: "Purchase is not valid." };
        }
    } catch (error) {
        throw new https.HttpsError("internal", `Firebase Functions 오류: ${error.message}`);
    }
});

exports.updateTicketsDaily = https.onRequest(async (req, res) => {
    try {
        const usersRef = admin.database().ref('users');
        const snapshot = await usersRef.once('value');

        snapshot.forEach(userSnapshot => {
            const userId = userSnapshot.key;
            const ticketDataRef = usersRef.child(`${userId}/_ticketData`);

            ticketDataRef.once('value', (ticketSnapshot) => {
                const updates = {};
                ticketSnapshot.forEach(ticket => {
                    if (ticket.val() === 0) {
                        updates[ticket.key] = 1;
                    }
                });

                if (Object.keys(updates).length > 0) {
                    ticketDataRef.update(updates);
                }
            });
        });

        console.log('✅ All user ticketData updated successfully.');
        res.status(200).send('Tickets updated successfully.');
    } catch (error) {
        console.error('🔥 Error updating tickets:', error);
        res.status(500).send('Error updating tickets');
    }
});
