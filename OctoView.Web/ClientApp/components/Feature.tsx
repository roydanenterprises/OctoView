import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface IFeatureProperties {
	name : string
}

interface IFeatureState {

}

export default class Feature extends
React.Component<RouteComponentProps<IFeatureProperties>, IFeatureState> {
    public render() {
        return <div>
            
        </div>;
    }
}
