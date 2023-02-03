export interface HttpRequest {
  method: string;
  url: string;
  contentType?: string;
  content?: any;
}

export interface FeatureFlag {
    status: string;
    value: {
        id: string;
        description: string;
        enabled: boolean;
    }
}

export interface FeatureSetting {
    id: string;
    value: string;
    description: string;
}
