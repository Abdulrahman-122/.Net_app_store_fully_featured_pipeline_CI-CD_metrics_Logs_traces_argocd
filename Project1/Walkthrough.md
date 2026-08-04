# Before you go with this project you must be familiar with
```
1.Python
2.bash
3.docker
4.kubernetes
5.Aws
6.Prometheus
7.Grafana
```

# How to Build kind cluster
go to this folder :  Prometh_Grafana/Project2_kind
and then do the following commands:
```
kind create cluster --config ../../../kind-config.yaml

```
then you need to download the traefik loadbalancer using helm chart:
```
helm repo add traefik https://traefik.github.io/charts
helm repo update
helm install my-traefik traefik/traefik \
  -n traefik \
  --create-namespace
 note:
   - make sure everything work well : kubectl get svc -n traeefik  
         - kubectl get pods -n traefik
         
install kube-prometheus-stack

```bash
helm repo add prometheus-community \
https://prometheus-community.github.io/helm-charts

helm repo update
```

Install

```bash
helm install monitoring \
prometheus-community/kube-prometheus-stack \
-n monitoring \
--create-namespace
```

Wait until

```bash
kubectl get pods -n monitoring
```
now move to this folder inside that parent folder Project2_kind  
and start install the chart in order to deploy the cluster
```
cd kind/flask-chart
helm install development(or any name)  .
once you did it , it will start.
```
now test the application whether it will be open or not using traefik
```
make sure -> vim /etc/hosts 
then put: 
127.0.0.1  flask.local
then port the traffic to the browser to see the applicaion:
k port-forward svc/traefik -n traefik  8080:80  
on your browser put : http://flask.local:8080
now register and write a post 
then now write: http://flask.local:8000/metrics 
now you will see  something like this: 
```
<img width="1305" height="916" alt="image" src="https://github.com/user-attachments/assets/172f4fde-b21f-4763-ac4f-f9881d8d7828" />

Now let's start our Prometheus+Grafana stuff:
```
for Prometheus:
k port-forward svc/monitoring-kube-prometheus-prometheus -n monitoring 9090:9090
then make sure it detect development-pod/metrics + other resources on the cluster like:
```
- <img width="684" height="68" alt="image" src="https://github.com/user-attachments/assets/13ff252a-a288-4a19-a5e8-ed107101ce44" />
- <img width="1830" height="948" alt="image" src="https://github.com/user-attachments/assets/821c4728-4ba0-4f58-a268-dba27b762773" />
- you will notice the first target is the name of the servicemonitor.yaml file that we wrote.

For Grafana
```
forward the traffic on port 3000
kubectl port-forward svc/monitoring-grafana \
3000:80 \
-n monitoring
now move to browser: http://localhost:3000 
then for the user: admin
the password you need to find it from the kubernetes secret itself:
kubectl get secret monitoring-grafana -n monitoring \
-o jsonpath="{.data.admin-password}" | base64 -d 
once you did it 
you can now  apply and build that promql commands on a new dashboard 
or 
just see mine (this is for application): http://localhost:3000/public-dashboards/a58e971c0aa74d4cbbe2a1e9ef17ed02
for the cluster itself see my mine : http://localhost:3000/public-dashboards/7537204ce5164c3b8d16c02a8a4a6d59 
```
-<img width="1744" height="618" alt="image" src="https://github.com/user-attachments/assets/2ee7f9e7-1e8b-415c-a289-a1a16f9cd610" />
- 
- <img width="1699" height="776" alt="image" src="https://github.com/user-attachments/assets/4e53ffcc-1b63-46bb-8ed9-e0291da3c442" />


- untill now this is good once you did this entire project: you will be good at Devops stuff.



# EKS Cluster on aws:
```
you must have an account on aws before you start and you have the free tier eligible or whatever paid stuff(in my case iam free tier so i will choose specific instances)
1. run this to see the specific instance:aws ec2 describe-instance-types --filte  
rs Name=free-tier-eligible,Values=true --query  
"InstanceTypes[].InstanceType"
2.now start build the cluster:
eksctl create cluster \
  --name flask-cluster \    -> random name
  --region eu-west-3        -> put your AZ instead of mine
  --version 1.36 \          --> note choose the last version
  --nodegroup-name workers \ -> random name just to make a whole name for the entire nodes
  --node-type c7i-flex.large  \  -> gives you 2cpu,4Gi memory
  --nodes 2 \                    -> choose your specific node 
  --managed                      -> in order to make sure it will be managed by aws not you but you can ignore that line

  3. install OIDC(open connection with cloud from your cluster)
    Associate the IAM OIDC provider.

```bash
eksctl utils associate-iam-oidc-provider \
--cluster flask-cluster \
--region eu-west-3 \
--approve
```

This allows Kubernetes ServiceAccounts to assume IAM roles.

Without it

```
ALB Controller

↓

cannot call AWS APIs
```

---

## Step 2

Download the IAM policy.

```bash
curl -O https://raw.githubusercontent.com/kubernetes-sigs/aws-load-balancer-controller/main/docs/install/iam_policy.json
```

---

## Step 3

Create the IAM policy.

```bash
aws iam create-policy \
--policy-name AWSLoadBalancerControllerIAMPolicy \
--policy-document file://iam_policy.json
```
## Step 4

Create the ServiceAccount.

```bash
eksctl create iamserviceaccount \
--cluster flask-cluster \
--namespace kube-system \
--name aws-load-balancer-controller \
--attach-policy-arn arn:aws:iam::<ACCOUNT_ID>:policy/AWSLoadBalancerControllerIAMPolicy \
--approve
```
```
- Varify the pod identity
aws eks describe-addon \
    --cluster-name flask-cluster \
    --addon-name aws-ebs-csi-driver
#if not found create one    
eksctl create addon \
    --cluster flask-cluster \
    --name eks-pod-identity-agent
eksctl create podidentityassociation \
  --cluster flask-cluster \
  --namespace kube-system \
  --service-account-name aws-load-balancer-controller \
  --permission-policy-arns arn:aws:iam::<Account-ID>:policy/AWSLoadBalancerControllerIAMPolicy
- crease this addon(aws-ebs-csi-driver) in order to connect vpc to vp on aws cloud . 
  eksctl create addon \
  --cluster flask-cluster \
  --name aws-ebs-csi-driver \
  --force
```
Now let's install Amazon loadbalancer(ALB) using Helm:
```
helm repo add eks https://aws.github.io/eks-charts

helm repo update

helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
-n kube-system \
--set clusterName=flask-cluster \
--set serviceAccount.create=false \
--set serviceAccount.name=aws-load-balancer-controller \
--set region=eu-west-3 \
--set vpcId=<YOUR_VPC_ID>

#to know your vpc-id
aws eks describe-cluster --name flask-cluster --region eu-west-3 --query "cluster.resourcesVpcConfig.vpcId" --output text
```
Now verify that ALB works well:
```
kubectl get pods -n kube-system
You should see


aws-load-balancer-controller

Running
```
now  run the application: (you must inside that folder ***cd project1/Prometh_Grafana/Project3_aws/***) 

-install kube-prometheus-stack

Exactly like Kind.

```bash
helm repo add prometheus-community \
https://prometheus-community.github.io/helm-charts

helm repo update
```

- Install

```bash
helm install monitoring \
prometheus-community/kube-prometheus-stack \
-n monitoring \
--create-namespace
```

Wait until

```bash
kubectl get pods -n monitoring

helm install flask ./flask-chart \
-n my-ns
```
note:
 Verify the Ingress

This is where the magic happens.

Instead of

```
localhost
```

you'll get

```
AWS DNS

↓

xxxxxxxx.eu-west-3.elb.amazonaws.com
```

Run

```bash
kubectl get ingress -n my-ns
```

Eventually

ADDRESS

k8s-flask-xxxxxxxx.eu-west-3.elb.amazonaws.com
```
in my case:
- <img width="1136" height="105" alt="image" src="https://github.com/user-attachments/assets/f53b48d6-c0fa-486c-84f1-7e13a7c69230" />
- just take that dns and start it from your browser but wait some minutes in order to see the project.
```
Now let's start our Prometheus+Grafana stuff:


for Prometheus:

```
k port-forward svc/monitoring-kube-prometheus-prometheus -n monitoring 9090:9090
then make sure it detect development-pod/metrics + other resources on the cluster like:
```

- <img width="684" height="68" alt="image" src="https://github.com/user-attachments/assets/13ff252a-a288-4a19-a5e8-ed107101ce44" />
- <img width="1830" height="948" alt="image" src="https://github.com/user-attachments/assets/821c4728-4ba0-4f58-a268-dba27b762773" />
- you will notice the first target is the name of the servicemonitor.yaml file that we wrote.


For Grafana
```
forward the traffic on port 3000
kubectl port-forward svc/monitoring-grafana \
3000:80 \
-n monitoring
```
```
now move to browser: http://localhost:3000 

then for the user: admin

the password you need to find it from the kubernetes secret itself:

kubectl get secret monitoring-grafana -n monitoring \
-o jsonpath="{.data.admin-password}" | base64 -d 

```
```
once you did it 
you can now  apply and build that promql commands on a new dashboard 
or 
just see mine (this is for application): http://localhost:3000/public-dashboards/a58e971c0aa74d4cbbe2a1e9ef17ed02
for the cluster itself see my mine : http://localhost:3000/public-dashboards/7537204ce5164c3b8d16c02a8a4a6d59 
```

-<img width="1744" height="618" alt="image" src="https://github.com/user-attachments/assets/2ee7f9e7-1e8b-415c-a289-a1a16f9cd610" />
- 
- <img width="1699" height="776" alt="image" src="https://github.com/user-attachments/assets/4e53ffcc-1b63-46bb-8ed9-e0291da3c442" />


- untill now this is good once you did this entire project: you will be good at Devops stuff.

