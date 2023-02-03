import React, { useState, useEffect, Fragment } from "react";
import axios from "axios";

import Card from "@material-ui/core/Card";
import Typography from "@material-ui/core/Typography";
import CardContent from "@material-ui/core/CardContent";
import { useMutation } from "react-query";

import settings from "../config/settings";

import { MessageDialog } from "../components/MessageDialog";
import { FeatureFlagsTable } from "../components/FeatureFlagsTable";
import { SkeletonFeatureFlag } from "../components/SkeletonFeatureFlag";

import { getToken } from "../services/authorization-service";

const featureFlagsPage: React.FunctionComponent = () => {
    const [loaded, setLoaded] = useState(false);
    const [featureFlags, setFeatureFlags] = useState([]);
    const [error, setError] = useState("");

    const fetchFeatureFlagsAsync = async () => {
        const { data } = await axios.get(`${settings.endpointUrl}/AppConfiguration/FeatureFlags`);

        setFeatureFlags(data);

        setLoaded(true);
    }
    
    const mutation: any = useMutation((key: string) => axios.post(`${settings.endpointUrl}/AppConfiguration/FeatureFlags/${key}/Toggle`, {},
            {
                headers: {
                    Authorization: getToken()
                }
            }),
        {
            onSuccess: (e) => {
                fetchFeatureFlagsAsync();
            },
            onError: (err: any) => {
                if (err.response.status === 401) {
                    const message = getToken().length === 0
                        ? "Unauthorized - Required Token"
                        : "Unauthorized - Invalid Token";
                    setError(message);
                } else {
                    setError("An unexpected error has occurred");
                }
                
                fetchFeatureFlagsAsync();
            }
        });

    useEffect(() => {
        fetchFeatureFlagsAsync();
    }, []);

    return (
        <Fragment>
            <article className="acm-feature-flags">
                <Card>
                    <CardContent style={{ padding: 16 }}>
                        <Typography variant="h5">Feature Flag Manager</Typography>
                        {loaded ? (<FeatureFlagsTable
                            data={featureFlags}
                            onChange={(id: string) => {
                                mutation.mutate(id);
                            }}
                        />) : (<SkeletonFeatureFlag />)}
                    </CardContent>
                </Card>
            </article>
            <MessageDialog
                open={error.length > 0}
                title="Something is wrong :("
                message={error}
                onClose={() => {
                    setError("");
                }}/>
        </Fragment>
);

};

export { featureFlagsPage as FeatureFlagsPage };