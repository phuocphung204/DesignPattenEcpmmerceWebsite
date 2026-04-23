import http from "@/services/http";
import { buildPath } from "@/utils/build-path-query-param";

const ADDRESS_API_BASE_URL = "https://provinces.open-api.vn/api/v1";

export type VietNamDivisionType =
	| "tỉnh"
	| "thành phố trung ương"
	| "huyện"
	| "quận"
	| "thành phố"
	| "thị xã"
	| "xã"
	| "thị trấn"
	| "phường";

export type Ward = {
	name: string;
	code: number;
	division_type: VietNamDivisionType;
	codename: string;
	district_code: number;
};

export type District = {
	name: string;
	code: number;
	division_type: VietNamDivisionType;
	codename: string;
	province_code: number;
	wards?: Ward[];
};

export type Province = {
	name: string;
	code: number;
	division_type: VietNamDivisionType;
	codename: string;
	phone_code: number;
	districts?: District[];
};

// phong bì dữ liệu chung cho API trả về
type DataEnvelope<T> = {
	data: T;
};

const unwrapPayload = <T>(payload: unknown): T => {
  // nếu payload có dạng { data: ... } thì trả về phần data, ngược lại trả thẳng payload
	if (
		payload &&
		typeof payload === "object" &&
		"data" in payload &&
		(payload as DataEnvelope<T>).data !== undefined
	) {
		return (payload as DataEnvelope<T>).data;
	}

	return payload as T;
}

const addressApi = {
  listProvinces: async () => {
    "use cache";

		const response = await http.get<Province[]>("/p/", {
			baseUrl: ADDRESS_API_BASE_URL,
		});

		return unwrapPayload<Province[]>(response.payload);
	},

  getProvince: async (code: number, depth: 1 | 2 | 3 = 1) => {
    "use cache";

		const response = await http.get<Province>(
			buildPath(`/p/${code}`, { depth }),
			{
				baseUrl: ADDRESS_API_BASE_URL,
			},
		);

		return unwrapPayload<Province>(response.payload);
	},

  getDistrict: async (code: number, depth: 1 | 2 = 1) => {
    "use cache";
    
		const response = await http.get<District>(
			buildPath(`/d/${code}`, { depth }),
			{
				baseUrl: ADDRESS_API_BASE_URL,
			},
		);

		return unwrapPayload<District>(response.payload);
	},
};

export default addressApi;
