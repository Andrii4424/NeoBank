import { Routes } from '@angular/router';
import { Layout } from './common-ui/layout/layout';
import { BankInfo } from './bank-info/bank-info';
import { Home } from './pages/home/home';
import { Login } from './pages/auth/login/login';

export const routes: Routes = [
    {path: '', component: Layout, children: [
        {path: '', component: Home}
    ]},
    {path: 'login', component: Login}

];
