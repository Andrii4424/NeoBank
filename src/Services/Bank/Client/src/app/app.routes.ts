import { Routes } from '@angular/router';
import { Layout } from './common-ui/layout/layout';
import { Home } from './pages/home/home';
import { Login } from './pages/auth/login/login';
import { Register } from './pages/auth/register/register';
import { BankInfo } from './pages/products/bank/bank-info/bank-info';
import { UpdateBank } from './pages/products/bank/update-bank/update-bank';
import { CanActivateAuth } from './auth/access.guard';
import { UsersOwnProfile } from './pages/profile/users-own-profile/users-own-profile';
import { adminGuard } from './auth/admin.guard';
import { CardTariffs } from './pages/cards/card-tariffs/card-tariffs';
import { UserCards } from './pages/cards/user-cards/user-cards';
import { CardsLayout } from './pages/cards/cards-layout/cards-layout';

export const routes: Routes = [
    {path: '', component: Layout, children: [
        {path: '', component: Home},
        {path: 'about', component: BankInfo },
        {path: 'about/update', component: UpdateBank, canActivate: [adminGuard]},
        {path: 'my-profile', component: UsersOwnProfile, canActivate: [CanActivateAuth]},
        {path: 'cards', component: CardTariffs},
        {path: 'cards/my-cards', component: UserCards, canActivate: [CanActivateAuth]}
    ]},
    {path: 'login', component: Login},
    {path: 'signup', component: Register}

];
