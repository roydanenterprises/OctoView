import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import * as BranchComponent from './Branch';

interface IBranches {
	branches: BranchComponent.IBranch[];
	loading: boolean;
}

export class Branches extends React.Component<RouteComponentProps<{}>, IBranches> {
	constructor() {
		super();
		this.state = { branches: [], loading: true };

		fetch('api/github/branches', {credentials: 'same-origin'})
			.then(response => response.json() as Promise<BranchComponent.IBranch[]>)
			.then(data => {
				this.setState({ branches: data, loading: false });
			});
	}

	render() {
		const contents = this.state.loading
			? <p>
				  <em>Loading...</em>
			  </p>
			: Branches.renderBranches(this.state.branches);

		return <div>
			       <h1>Branches</h1>
			       {contents}
		       </div>;
	}

	private static renderBranches(branches: BranchComponent.IBranch[]) {
		return <div>
			       {branches.map(branch => <BranchComponent.Branch layout="notTable" {...branch}/>)}
		       </div>;
	}
}