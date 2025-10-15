import { Currency } from "../../../../enums/currency";
import { TransactionType } from "../../../../enums/transaction-type";

export interface IAddFunds{
    cardId: string;
    amount: number;
    operationType : TransactionType;
    cardCurrency : Currency;
}