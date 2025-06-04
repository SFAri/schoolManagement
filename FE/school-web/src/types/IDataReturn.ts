export interface IDataReturn<T> {
    data : T | null;
    error: string | null;
    loading: boolean
}