import React, { FunctionComponent } from "react"
import { Route, Redirect, Switch } from "react-router-dom";

import { FeatureFlagsPage } from "../pages/FeatureFlagsPage";
import { FeatureSettingsPage } from "../pages/FeatureSettingsPage";

const routes: FunctionComponent = () => {
    return (
        <Switch>
            <Route
                exact
                path="/"
                render={() => <Redirect to="/feature-flags" />}
            />
            <Route
                path="/feature-flags"
                render={() => <FeatureFlagsPage />}
            />
            <Route
                path="/settings"
                render={() => <FeatureSettingsPage />}
            />
        </Switch>
    );
};

export default routes;