import ProductCategoryType from "@/entities/product-category/type";
import http from "@/services/http";

const productCategoryRequest = {
  getLookupList: async () => {
    "use cache";

    return http.get<ProductCategoryType[]>(
      "/product-categories/lookup"
    );
  }
}

export default productCategoryRequest;