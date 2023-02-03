import React, { useState, useEffect, Fragment } from "react";
import axios from "axios";

import Card from "@material-ui/core/Card";
import Typography from "@material-ui/core/Typography";
import CardContent from "@material-ui/core/CardContent";
import { useMutation } from "react-query";

import settings from "../config/settings";

import { FeatureSettingsTable } from "../components/FeatureSettingsTable";
import { getToken } from "../services/authorization-service";
import { MessageDialog } from "../components/MessageDialog";


const featureSettingsPage: React.FunctionComponent = () => {
    const [loaded, setLoaded] = useState(false);
    const [featureSettings, setFeatureSettings] = useState([]);
    const [error, setError] = useState("");

    const fetchFeatureSettingsAsync = async () => {
        const { data } = await axios.get(`${settings.endpointUrl}/AppConfiguration/Settings`);

        setFeatureSettings(data);

        setLoaded(true);
    }

    const mutation: any = useMutation(({ key, value }: { key: string, value: string }) => axios.post(`${settings.endpointUrl}/AppConfiguration/Settings/${key}/${value}`, {},
            {
                headers: {
                    Authorization: getToken()
                }
            }),
        {
            onSuccess: (e) => {
                fetchFeatureSettingsAsync();
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

                fetchFeatureSettingsAsync();
            }
        });

    useEffect(() => {
        fetchFeatureSettingsAsync();
    }, []);
    
    return (
        <Fragment>
            <article className="acm-feature-flags">
                <Card>
                    <CardContent style={{ padding: 16 }}>
                        <Typography variant="h5">Feature Settings Manager</Typography>
                        {loaded && featureSettings !== undefined && <FeatureSettingsTable data={featureSettings} onChange={(key: string, value: string) => {
                            mutation.mutate({ key, value });
                        }} />}
                    </CardContent>
                </Card>
            </article>
            <MessageDialog
                open={error.length > 0}
                title="Something is wrong :("
                message={error}
                onClose={() => {
                    setError("");
                }}
            />
        </Fragment>
    );

};

export { featureSettingsPage as FeatureSettingsPage };