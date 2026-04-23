import http from "@/services/http";
import { CreateOrderResponseType } from "@/services/client-requests/order-requests";

export type VnPayCallbackRequest = {
	vnp_TxnRef: string;
	vnp_Amount: number;
	vnp_TransactionNo: string;
	vnp_ResponseCode: string;
	vnp_BankCode: string;
	vnp_CardType: string;
	vnp_PayDate: string;
	vnp_OrderInfo: string;
}

export type MomoCallbackRequest = {
	orderId: string;
	requestId: string;
	amount: number;
	transId: number;
	resultCode: number;
	message: string;
	payType: string;
	responseTime: number;
	bankCode: string;
}

export type ProcessCallbackResponseType = CreateOrderResponseType;

const paymentRequests = {
	mockVnPayCallback(payload: VnPayCallbackRequest) {
		return http.post<ProcessCallbackResponseType>("/payments/mock/vnpay-callback", payload);
	},
	mockMomoCallback(payload: MomoCallbackRequest) {
		return http.post<ProcessCallbackResponseType>("/payments/mock/momo-callback", payload);
	},
};

export default paymentRequests;
