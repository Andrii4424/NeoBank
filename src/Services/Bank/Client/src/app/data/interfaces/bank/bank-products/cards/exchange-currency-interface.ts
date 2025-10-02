import { Currency } from "../../../../enums/currency";

export interface IExchangeCurrency{
    from: Currency ;
    to: Currency ;
    amount : number;
}