import { Routes } from '@angular/router';
import { Layout } from './common-ui/layout/layout';
import { BankInfo } from './bank-info/bank-info';

export const routes: Routes = [
    {path: '', component: Layout},
    {path: 'BankInfo', component: BankInfo}

];
