export function isInRole(profile, role)
{
  if (!profile) return false
  return Array.isArray(profile) ? (profile.filter(i => i === role).length > 0) : (profile === role) 
}

export function isSelf(user, memberId) {
   const r = memberId ? (!!(user && user.access_token) && user.profile && user.profile.memberId &&user.profile.memberId.toLowerCase() === memberId.toLowerCase()) : false
  // console.log(r)
   return r
  //return true
}