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
import { UserCards } from './pages/cards/user-cards/user-cards';
import { CardsLayout } from './pages/cards/cards-layout/cards-layout';
import { CardTariffsInfo } from './pages/cards/tariffs/card-tariffs-info/card-tariffs-info';
import { CardTariffs } from './pages/cards/tariffs/card-tariffs/card-tariffs';
import { UpdateTariffs } from './pages/cards/tariffs/update-tariffs/update-tariffs';

export const routes: Routes = [
    {path: '', component: Layout, children: [
        {path: '', component: Home},
        {path: 'about', component: BankInfo },
        {path: 'about/update', component: UpdateBank, canActivate: [adminGuard]},
        {path: 'my-profile', component: UsersOwnProfile, canActivate: [CanActivateAuth]},
        {path: 'cards', component: CardTariffs},
        {path: 'cards/my-cards', component: UserCards, canActivate: [CanActivateAuth]},
        {path: 'cards/info/:id', component: CardTariffsInfo},
        {path: 'cards/update/:id', component: UpdateTariffs, canActivate: [adminGuard]}
    ]},
    {path: 'login', component: Login},
    {path: 'signup', component: Register}

];
