export const buildPath = (path: string, query?: Record<string, string | number | null>) => {
	if (!query) {
		return path;
	}

	const searchParams = new URLSearchParams();

	Object.entries(query).forEach(([key, value]) => {
		if (value !== null) {
			searchParams.append(key, String(value));
		}
	});

	const queryString = searchParams.toString();
	return queryString ? `${path}?${queryString}` : path;
};