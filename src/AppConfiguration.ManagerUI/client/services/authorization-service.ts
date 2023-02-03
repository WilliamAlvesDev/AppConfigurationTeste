const tokenKey = "appconfigation:token";

export const getToken = () => {
    return sessionStorage.getItem(tokenKey) ?? "";
}

export const storeToken = (token: string) => sessionStorage.setItem(tokenKey, token);

export const removeToken = () => sessionStorage.removeItem(tokenKey);