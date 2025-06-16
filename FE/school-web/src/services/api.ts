import axios from './custom-axios';
import { AxiosRequestConfig } from 'axios';

// ---------- API --------------
export const getReq = async<T> (url: string, params?: Record<string, unknown>, signal?: AbortSignal): Promise<T> => {
    const config: AxiosRequestConfig = {
        params,
        signal
    };

    const response = await axios.get<T>(url, config);
    return response.data;
};


export const postReq = async<T> (url: string, data: Record<string, unknown>, signal?: AbortSignal): Promise<T> => {
    const config: AxiosRequestConfig = {
        signal
    };

    const response = await axios.post<T>(url, data, config);
    return response.data;
};

export const putReq = async<T> (url: string, data: Record<string, unknown>, signal?: AbortSignal): Promise<T> => {
    const config: AxiosRequestConfig = {
        signal
    };

    const response = await axios.put<T>(url, data, config);
    return response.data;
};

export const patchReq = async<T> (url: string, data: Record<string, unknown>, signal?: AbortSignal): Promise<T> => {
    const config: AxiosRequestConfig = {
        signal
    };

    const response = await axios.patch<T>(url, data, config);
    return response.data;
};

export const deleteReq = async<T> (url: string, signal?: AbortSignal): Promise<T> => {
    const config: AxiosRequestConfig = {
        signal
    };

    const response = await axios.delete<T>(url, config);
    return response.data;
};
