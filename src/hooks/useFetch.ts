import { useState, useEffect } from 'react';
import { getReq, postReq, patchReq, deleteReq } from '../services/api';
import { IDataReturn } from '../types/IDataReturn';
import axios from 'axios';

const useFetch = <T>(
    url: string,
    params?: Record<string, unknown>,
    method: 'GET' | 'POST' | 'PATCH' | 'DELETE' = 'GET',
    data?: Record<string, unknown>
): IDataReturn<T> => {
    const [dataResult, setData] = useState<T | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(()=>{
        const controller = new AbortController();
        const signal = controller.signal;
        console.log("Signal: ", signal)

        const fetchData = async () => {
            try {
                setLoading(true);
                let result;
                console.log("ENTER TRY")

                // If case method:
                if (method === 'GET'){
                    result = await getReq<T>(url, params ,signal)
                }
                else if ( method === 'POST'){
                    result = await postReq<T>(url, data||{} ,signal)
                }
                else if (method === 'PATCH'){
                    result = await patchReq<T>(url, data||{} ,signal)
                }
                // Delete:
                else {
                    result = await deleteReq<T>(url, signal)
                }
                setData(result);
            }
            catch(err){
                if (axios.isCancel(err)){
                    console.log('Request canceled: ', err.message);
                }
                else {
                    setError(`Error: ${err}`)
                }
            }
            finally {
                setLoading(false);
            }
        };

        fetchData();

        return () => {
            controller.abort();
        }
    }, [data, method, url]);
    return {data: dataResult, error, loading};
}

export default useFetch;