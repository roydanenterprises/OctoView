import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface IFeatureData {
	name: string;
}

interface IFeaturesState {
	features: IFeatureData[];
	loading: boolean;
}


export class Features extends React.Component<RouteComponentProps<{}>, IFeaturesState> {
	constructor() {
		super();
		this.state = { features: [], loading: true };

		fetch('api/github/fakeBranches')
			.then(response => response.json() as Promise<IFeatureData[]>)
			.then(data => {
				this.setState({ features: data, loading: false });
			});
	}

	render() {
		let contents = this.state.loading
			? <p><em>Loading...</em></p>
			: Features.renderFeatures(this.state.features);

		return <div>
			       <h1>Features</h1>
			       {contents}
		       </div>;
	}

	private static renderFeatures(features: IFeatureData[]) {
		return <table className="table">
			       <thead>
			       <tr>
					<th>Name</th>
					<th>Foo data</th>
			       </tr>
			       </thead>
			       <tbody>
				{features.map(feature =>
					<tr key={feature.name}>
						<td>{feature.name}</td>
						<td>Foo Data</td>
				       </tr>
			       )}
			       </tbody>
		       </table>;
	}
}