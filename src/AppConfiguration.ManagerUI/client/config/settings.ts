
export type Settings = {
    endpointUrl: string;
}

const settings = ({
    endpointUrl: !window.endpointUrl ? `${window.location.origin}/api` : `${window.location.origin}/${window.endpointUrl}/api`,
}) as Settings;

export default settings;