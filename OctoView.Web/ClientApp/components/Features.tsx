import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import * as FeatureComponent from './Feature';

interface IFeatures {
	features: FeatureComponent.IFeature[];
	loading: boolean;
}



export class Features extends React.Component<RouteComponentProps<{}>, IFeatures> {
	constructor() {
		super();
		this.state = { features: [], loading: true };

		fetch('api/github/fakeBranches')
			.then(response => response.json() as Promise<FeatureComponent.IFeature[]>)
			.then(data => {
				this.setState({ features: data, loading: false });
			});
	}

	render() {
		const contents = this.state.loading
			? <p>
				  <em>Loading...</em>
			  </p>
			: Features.renderFeatures(this.state.features);

		return <div>
			       <h1>Features</h1>
			       {contents}
		       </div>;
	}

	private static renderFeatures(features: FeatureComponent.IFeature[]) {
		return <div>
			{features.map(feature => <FeatureComponent.Feature layout="notTable" {...feature}/>)}
		       </div>;
	}
}