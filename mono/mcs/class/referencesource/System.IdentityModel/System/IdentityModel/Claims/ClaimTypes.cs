//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.IdentityModel.Claims
{
    public static class ClaimTypes
    {
        const string claimTypeNamespace = XsiConstants.Namespace + "/claims";

        const string anonymous = claimTypeNamespace + "/anonymous";
        const string dns = claimTypeNamespace + "/dns";
        const string email = claimTypeNamespace + "/emailaddress";
        const string hash = claimTypeNamespace + "/hash";
        const string name = claimTypeNamespace + "/name";
        const string rsa = claimTypeNamespace + "/rsa";
        const string sid = claimTypeNamespace + "/sid";
        const string denyOnlySid = claimTypeNamespace + "/denyonlysid";
        const string spn = claimTypeNamespace + "/spn";
        const string system = claimTypeNamespace + "/system";
        const string thumbprint = claimTypeNamespace + "/thumbprint";
        const string upn = claimTypeNamespace + "/upn";
        const string uri = claimTypeNamespace + "/uri";
        const string x500DistinguishedName = claimTypeNamespace + "/x500distinguishedname";

        const string givenname = claimTypeNamespace + "/givenname";
        const string surname = claimTypeNamespace + "/surname";
        const string streetaddress = claimTypeNamespace + "/streetaddress";
        const string locality = claimTypeNamespace + "/locality";
        const string stateorprovince = claimTypeNamespace + "/stateorprovince";
        const string postalcode = claimTypeNamespace + "/postalcode";
        const string country = claimTypeNamespace + "/country";
        const string homephone = claimTypeNamespace + "/homephone";
        const string otherphone = claimTypeNamespace + "/otherphone";
        const string mobilephone = claimTypeNamespace + "/mobilephone";
        const string dateofbirth = claimTypeNamespace + "/dateofbirth";
        const string gender = claimTypeNamespace + "/gender";
        const string ppid = claimTypeNamespace + "/privatepersonalidentifier";
        const string webpage = claimTypeNamespace + "/webpage";
        const string nameidentifier = claimTypeNamespace + "/nameidentifier";
        const string authentication = claimTypeNamespace + "/authentication";
        const string authorizationdecision = claimTypeNamespace + "/authorizationdecision";

        public static string Anonymous
        {
            get { return anonymous; }
        }
        public static string DenyOnlySid
        {
            get { return denyOnlySid; }
        }
        public static string Dns
        {
            get { return dns; }
        }
        public static string Email
        {
            get { return email; }
        }
        public static string Hash
        {
            get { return hash; }
        }
        public static string Name
        {
            get { return name; }
        }
        public static string Rsa
        {
            get { return rsa; }
        }
        public static string Sid
        {
            get { return sid; }
        }
        public static string Spn
        {
            get { return spn; }
        }
        public static string System
        {
            get { return system; }
        }
        public static string Thumbprint
        {
            get { return thumbprint; }
        }
        public static string Upn
        {
            get { return upn; }
        }
        public static string Uri
        {
            get { return uri; }
        }
        public static string X500DistinguishedName
        {
            get { return x500DistinguishedName; }
        }
        public static string NameIdentifier
        {
            get { return nameidentifier; }
        }
        public static string Authentication
        {
            get { return authentication; }
        }
        public static string AuthorizationDecision
        {
            get { return authorizationdecision; }
        }

        // used in info card
        static public string GivenName
        {
            get { return givenname; }
        }
        public static string Surname
        {
            get { return surname; }
        }
        public static string StreetAddress
        {
            get { return streetaddress; }
        }
        public static string Locality
        {
            get { return locality; }
        }
        public static string StateOrProvince
        {
            get { return stateorprovince; }
        }
        public static string PostalCode
        {
            get { return postalcode; }
        }
        public static string Country
        {
            get { return country; }
        }
        public static string HomePhone
        {
            get { return homephone; }
        }
        public static string OtherPhone
        {
            get { return otherphone; }
        }
        public static string MobilePhone
        {
            get { return mobilephone; }
        }
        public static string DateOfBirth
        {
            get { return dateofbirth; }
        }
        public static string Gender
        {
            get { return gender; }
        }
        public static string PPID
        {
            get { return ppid; }
        }
        public static string Webpage
        {
            get { return webpage; }
        }
    }
}
