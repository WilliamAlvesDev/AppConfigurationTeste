import React from "react";
import { NavLink } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "react-query";

import { AsideMenu } from "./components/AsideMenu";
import { Header } from "./components/Header";
import { SettingsIcon, FeatureFlagsIcon } from "./components/icons/index";
import Routes from "./routes/Routes";

const queryClient = new QueryClient();

const app = () => (
    <QueryClientProvider client={queryClient}>
        <main id="outer-container">
            <AsideMenu>
                <NavLink
                    to="/feature-flags"
                    className="acm-aside-menu__item"
                    activeClassName="acm-aside-menu__item--active"
                >
                    <FeatureFlagsIcon />
                    <span>Feature Flags</span>
                </NavLink>
                <NavLink
                    to="/settings"
                    className="acm-aside-menu__item"
                    activeClassName="acm-aside-menu__item--active"
                >   <SettingsIcon />
                    <span>Settings</span>
                </NavLink>
            </AsideMenu>

            <section className="acm-section-router">
                <Header />
                <Routes />
            </section>
        </main>
    </QueryClientProvider>
);

export { app as App };
