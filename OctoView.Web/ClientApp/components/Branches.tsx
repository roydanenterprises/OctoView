import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import * as BranchComponent from './Branch';
import { Branch } from './Branch';
import { PullRequest } from 'ClientApp/components/PullRequest';
import { CodeReview } from './CodeReview';

interface IBranches {
	branches: BranchComponent.IBranch[];
	loading: boolean;
	filter: string;
	user: IGithubUser;
}

interface IGithubUser {
	username: string;
	avatarUrl: string;
}

export class Branches extends React.Component<RouteComponentProps<{}>, IBranches> {
	constructor() {
		super();
		this.state = { branches: [], loading: true, filter: "noFilter", user: { username: "", avatarUrl:"" }};
	}

	componentWillMount() {
		fetch('api/github/branches', { credentials: 'same-origin' })
			.then(response => response.json() as Promise<BranchComponent.IBranch[]>)
			.then(data => {
				this.setState({ branches: data, loading: false });
			});
		fetch('api/github/user', { credentials: 'same-origin' })
			.then(response => response.json() as Promise<IGithubUser>)
			.then(data => {
				this.setState({ user: data });
			});
	}

	handleFilterChange(changeEvent: { target: HTMLInputElement; }) {
		this.setState({
			filter: changeEvent.target.value
		});
	}

	renderBranches() {
		let localBranches = this.state.branches;
		if (this.state.filter === 'assignToMe') {
			localBranches = localBranches.filter(branch => {
				return branch.pulls.some(pull => pull.assignee === this.state.user.username);
			});
		}
		return <div>
					{localBranches.map(branch => <BranchComponent.Branch layout="notTable" {...branch} />)}
				</div >;

	}

	render() {
		const contents = this.state.loading
			? <p>
				  <em>Loading...</em>
			  </p>
			: this.renderBranches();

		return	<div>
			<div>
						<div className="radio">
							<label>
						<input type="radio" value="noFilter" checked={this.state.filter === 'noFilter'} onChange={(e) => this.handleFilterChange(e)} />
								No Filter
									</label>
						</div>
						<div className="radio">
							<label>
						<input type="radio" value="assignToMe" checked={this.state.filter === 'assignToMe'} onChange={(e) => this.handleFilterChange(e)}/>
								Assign To Me
							</label>
						</div>
						<div className="radio">
							<label>
						<input type="radio" value="involvingMe" checked={this.state.filter === 'involvingMe'} onChange={(e) => this.handleFilterChange(e)}/>
								Involving Me
							</label>
						</div>
						<div className="radio">
							<label>
						<input type="radio" value="toDo" checked={this.state.filter === 'toDo'} onChange={(e) => this.handleFilterChange(e)}/>
								To Do
							</label>
						</div>
					</div>
					<div>
						<h1>Branches</h1>
						{contents}
					</div>
				</div>;
	}

	private static renderBranches(branches: BranchComponent.IBranch[]) {
		return <div>
			       {branches.map(branch => <BranchComponent.Branch layout="notTable" {...branch}/>)}
		       </div>;
	}
}