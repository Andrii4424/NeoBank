import { Currency } from "../../../../enums/currency";
import { TransactionType } from "../../../../enums/transaction-type";
import { TransactionStatus } from "../../../../enums/transaction-status";

export interface ITransaction{
    id?: string; 
    senderCardId?: string; 
    senderId?: string; 
    getterCardId?: string; 
    getterId?: string; 
    senderCurrency?: Currency;
    getterCurrency?: Currency;
    currencyExchangeCommission?: number; 
    amount: number; 
    commission?: number; 
    status?: TransactionStatus;
    type: TransactionType;
    transactionTime?: string;
}