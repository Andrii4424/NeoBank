export interface IFilter{
    filterName: string;
    id: string |null;
    description: string | null;
    value: any;
    chosen: boolean |null;
}