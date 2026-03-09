import { CreditStatus } from "../../../enums/credit-status";
import { Currency } from "../../../enums/currency";
import { ICreditTariffs } from "../bank-products/credits/credit-tariffs/credit-tariffs";

export interface UserCredit {
  id?: string;

  userId: string;
  creditTariffId: string;

  amount: number;
  termMonths: number;

  interestRate: number;

  currency: Currency;

  startDate: string;    
  endDate: string;

  status: CreditStatus;

  monthlyPayment: number;
  remainingDebt: number;
  currentMonthAmountDue: number;
  currentPaymentDate: string;
  
  createdAt: string;

  creditTariffs : ICreditTariffs | null;
}