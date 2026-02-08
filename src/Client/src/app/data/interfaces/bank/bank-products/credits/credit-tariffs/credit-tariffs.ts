import { Currency } from "../../../../../enums/currency";

export interface ICreditTariffs{
        id: string;
        name: string
        interestRate: number;
        minAmount: number;
        maxAmount: number;
        minTermMonths: number;
        maxTermMonths: number;
        enableCurrency: Currency[];
}