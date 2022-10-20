import SignIn from 'components/sign-in'
import BoardsAll from 'components/boards-all'
import BoardsDefault from 'components/boards-default'
import Sprints from 'components/sprints'
import Issues from 'components/issues'
import Estimates from 'components/estimates'
import store from './store'

const allowAnonymous = (to, from, next) => {
    if (!store.getters.isAuthenticated) {
        next();
        return;
    }

    next('/');
}

const allowAuthenticated = (to, from, next) => {
    if (store.getters.isAuthenticated) {
        next();
        return;
    }

    next('/signin');
}

export const routes = [
    { path: '/signin', name: 'signin', component: SignIn, beforeEnter: allowAnonymous },
    { path: '/', name: 'home', component: BoardsDefault, beforeEnter: allowAuthenticated },
    { path: '/boards', component: BoardsDefault, beforeEnter: allowAuthenticated },
    { path: '/board/all', name: 'boardsAll', component: BoardsAll, beforeEnter: allowAuthenticated },
    { path: '/board/:id', name: 'board', component: Sprints, beforeEnter: allowAuthenticated },
    { path: '/sprint/:id', name: 'sprint', component: Issues, beforeEnter: allowAuthenticated },
    { path: '/estimates/:id', name: 'estimates', component: Estimates, beforeEnter: allowAuthenticated },
    { path: '*', redirect: '/' }
]