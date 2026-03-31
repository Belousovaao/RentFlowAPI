const BASE_URL = "http://localhost:5102";

export const apiClient = {
  async get<T>(url: string): Promise<T> {
    const fullUrl = `${BASE_URL}${url}`;
    console.log("FULL URL:", fullUrl);

    const response = await fetch(fullUrl);

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error);
    }

    return response.json();
  },

  async post<T>(url: string, body: unknown): Promise<T> {
    const fullUrl = `${BASE_URL}${url}`;

    const response = await fetch(fullUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error);
    }

    return response.json();
  },
};