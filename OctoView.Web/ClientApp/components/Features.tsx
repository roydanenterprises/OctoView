import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface IFeatureData {
	
}

interface IFeaturesState {
	features: IFeatureData[];
	loading: boolean;
} 


export class Features extends React.Component<RouteComponentProps<{}>, IFeaturesState> {
	constructor() {
		super();
		this.state = {features: [], loading: true };

		fetch('api/github/features')
			.then(response => response.json() as Promise<IFeatureData[]>)
			.then(data => {
				this.setState({ features: data, loading: false });
			});
	} 

	render() {
		return <div>
			       <h2>Features</h2>
		       </div>;
	}
}